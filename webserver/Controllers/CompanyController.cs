using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using webserver.Data;
using webserver.Models;
using System.Net;

namespace webserver.Controllers;

[Route("api/v1/company")]
[ApiController]
public class CompanyController : ControllerBase {
    
    private readonly WebserverContext _context;
    private readonly UserManager<Company> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public CompanyController(WebserverContext context, UserManager<Company> userManager, RoleManager<IdentityRole> roleManager) {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    class CompanySummary {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    [HttpGet("{string:id}")]
    public IActionResult ReadCompany(string id) {
        var company = _context.Company.Find(id);

        if (company != null) {

            var response = new {
                data = new {
                    type = "Company[]",
                    id = company.Id,
                    attributes = new CompanySummary {
                        Id = company.Id,
                        UserName = company.UserName!,
                        Email = company.Email!,
                        PhoneNumber = company.PhoneNumber!,
                        Country = company.Country,
                        State = company.State,
                        City = company.City
                    }
                }
            };

            return Ok(response);
        }else{
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] Company model, string nwordpass) {
        
            var auxUser = await _userManager.FindByEmailAsync(model.Email!);

            if(auxUser!=null){
                return BadRequest("Email already registered!");
            }
            
                var company = new Company {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Country = model.Country,
                    State = model.State,
                    City = model.City
                };

                var result = await _userManager.CreateAsync(company, nwordpass);

                if (result.Succeeded) {

                    var roleExist = await _roleManager.RoleExistsAsync(Common.Company_Role);
                    if (!roleExist) {
                        await _roleManager.CreateAsync(new IdentityRole(Common.Company_Role));
                    }

                    await _userManager.AddToRoleAsync(company, Common.Company_Role);

                    var response = new {
                        data = new {
                            type = "Company",
                            id = model.Id,
                            attributes = new CompanySummary {
                                UserName = model.UserName!,
                                Email = model.Email!,
                                PhoneNumber = model.PhoneNumber!,
                                Country = model.Country,
                                State = model.State,
                                City = model.City
                            }
                        }
                    };

                    return Ok(response);
                }else{
                    return BadRequest("Failed to create company. Check your input data.");
                }
            
        
    }

    [HttpPatch("{string:id}")]
    public IActionResult UpdateCompany(string id, [FromBody] Company newCompany) {

        var existingCompany = _context.Company.Find(id);
        if (existingCompany == null) {
            return NotFound();
        }

        existingCompany.UserName = newCompany.UserName;
        existingCompany.Email = newCompany.Email;
        existingCompany.PhoneNumber = newCompany.PhoneNumber;
        existingCompany.Country = newCompany.Country;
        existingCompany.State = newCompany.State;
        existingCompany.City = newCompany.City;

        _context.SaveChanges();

        var response = new {
            data = new {
                type = "Company",
                id = existingCompany.Id,
                attributes = new CompanySummary {
                    Country = existingCompany.Country,
                    State = existingCompany.State,
                    City = existingCompany.City
                }
            }
        };

        return Ok(response);
    }

    [HttpDelete("{string:id}")]
    public IActionResult DeleteCompany(string id) {
        var comp=_context.Company.Find(id);
        if(comp==null){
            return NotFound();
        }

        var relatedRealEstates = _context.RealEstates.Where(re => re.CompanyId == comp.Id);

        _context.RealEstates.RemoveRange(relatedRealEstates);
        _context.Company.Remove(comp);

        _context.SaveChanges();

        return NoContent();
    }
}