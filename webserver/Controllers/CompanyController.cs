using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using webserver.Data;
using webserver.Models;
using System.Net;
using Microsoft.IdentityModel.Tokens;

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

    [HttpGet]
    public IActionResult ReadCompany(string id) {
        var company = _context.Company.Find(id);

        if (company != null) {

            var response = new {
                data = new {
                    type = "Company",
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

    [HttpGet]
    public IActionResult ReadCompanies(string? Country, string? State, string? City, int? offset, int? limit, string sort = "Name") {

        var companies = _context.Company.AsQueryable();

        if (!string.IsNullOrEmpty(City)) {
            companies = companies.Where(re => re.City == City);
        }
        if (!string.IsNullOrEmpty(State)) {
            companies = companies.Where(re => re.State == State);
        }
        if (!string.IsNullOrEmpty(Country)) {
            companies = companies.Where(re => re.Country == Country);
        }

        sort = sort.ToLower();
        switch (sort) {
            case "city":
                companies = companies.OrderBy(re => re.City);
                break;
            case "state":
                companies = companies.OrderBy(re => re.State);
                break;
            case "country":
                companies = companies.OrderBy(re => re.Country);
                break;
            default:
                companies = companies.OrderBy(re => re.UserName);
                break;
        }

        if(offset.HasValue){
            companies=companies.Skip(offset.Value);
        }
        if(limit.HasValue){
            companies=companies.Take(limit.Value);
        }

        var result = companies.ToList();

        if (result == null) {
            return NotFound();
        }else {

            var response = new {
                data = result.Select(re => new {
                    
                    type = "Company",
                    attributes = new CompanySummary {
                        Id = re.Id,
                        UserName = re.UserName!,
                        Email=re.Email!,
                        PhoneNumber=re.PhoneNumber!,
                        Country=re.Country,
                        State=re.State,
                        City=re.City
                    }
                })
            };

            return Ok(response);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] Company comp, string password) {

        // Check if the email is already registered
        var existingUser = await _userManager.FindByEmailAsync(comp.Email!);
        if (existingUser != null) {
            return BadRequest("Email already registered!");
        }

        var company = new Company {
            UserName = comp.UserName,
            Email = comp.Email,
            PhoneNumber = comp.PhoneNumber,
            Country = comp.Country,
            State = comp.State,
            City = comp.City
        };

        var result = await _userManager.CreateAsync(company, password);

        if (result.Succeeded) {

            var roleExists = await _roleManager.RoleExistsAsync(Common.Company_Role);
            if (!roleExists) {
                await _roleManager.CreateAsync(new IdentityRole(Common.Company_Role));
            }

            await _userManager.AddToRoleAsync(company, Common.Company_Role);
            return Ok("Company created successfully!");
        }else{
            return StatusCode(500, "Internal Server Error: Register Company Unsuccessful");
        }
    }

    [HttpPatch]
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

    [HttpDelete]
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