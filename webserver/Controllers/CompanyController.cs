using Microsoft.AspNetCore.Mvc;
using webserver.Data;
using webserver.Models;

namespace webserver.Controllers;

[Route("api/v1/company")]
[ApiController]
public class CompanyController : ControllerBase {
    
    private readonly WebserverContext _context;

    public CompanyController(WebserverContext context) {
        _context = context;
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