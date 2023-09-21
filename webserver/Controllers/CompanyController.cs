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
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    [HttpGet("{id}")]
    public IActionResult ReadCompanys(int id) {

        var company = _context.Company.Find(id);

        if(company != null){

            var result=new CompanySummary {
                Country = company.Country,
                State = company.State,
                City = company.City,
                UserName = company.UserName!,
                Email = company.Email!,
                PhoneNumber = company.PhoneNumber!
            };

            return Ok(result);
        }else{
            return NotFound();
        }
    }

    [HttpPatch("{string:id}")]
    public IActionResult UpdateCompany(string id, Company company) {

        var comp=_context.Company.Find(id);
        if(comp==null){
            return NotFound();
        }
        
        comp.Email=company.Email;
        comp.PhoneNumber=company.PhoneNumber;

        _context.SaveChanges();
        return Ok();
    }

    [HttpDelete("{string:id}")]
    public IActionResult DeleteCompany(string id) {
        var comp=_context.Company.Find(id);
        if(comp==null){
            return NotFound();
        }

        _context.Company.Remove(comp);
        _context.SaveChanges();

        return NoContent();
    }
}