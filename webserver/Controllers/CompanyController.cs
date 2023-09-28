using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webserver.Data;
using webserver.Models;
using webserver.Models.DTOs;

namespace webserver.Controllers;

[ApiController]
[Route("api/v1/company")]
public class CompanyController : ControllerBase {
    
    private readonly WebserverContext _context;
    private readonly UserManager<Company> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public CompanyController(WebserverContext context, UserManager<Company> userManager, RoleManager<IdentityRole> roleManager) {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("unique/{id}")]
    public IActionResult ReadCompany(string id) {

        var company = _context.Company.Find(id);
        if(company==null){
            return NotFound();
        }

        var response = new {
            data = new {
                type = "Company",
                attribute = new CompanyDTO (
                    company.Id, company.UserName!, company.Email!, company.PhoneNumber!,
                    company.Country, company.State, company.City
                )
            }
        };

        return Ok(response);
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

        //var result = companies.ToList();
        var result = companies.ToArray();
        
        if(result==null){
            return NotFound();
        }

        var response = new {
            data = result.Select(c => new {
                
                type = "Company[]",
                attributes = new CompanyDTO (
                    c.Id, c.UserName!, c.Email!, c.PhoneNumber!,
                    c.Country, c.State, c.City
                )
            })
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyDTO comp, string password) {

        // Check if the email is already registered
        var existingEmail = await _userManager.FindByEmailAsync(comp.Email);
        if (existingEmail != null) {
            return BadRequest("Email already registered!");
        }

        var existingUsername = await _userManager.FindByNameAsync(comp.UserName);
        if (existingUsername != null) {
            return BadRequest("UserName already registered!");
        }

        Company company = (Company)comp;

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
    public IActionResult UpdateCompany([FromBody] CompanyDTO newCompany) {

        var existingCompany = _context.Company.Find(newCompany.Id);
        if (existingCompany == null) {
            return NotFound();
        }

        existingCompany=(Company)newCompany;

        _context.SaveChanges();

        var response = new {
            data = new {
                type = "Company",
                attribute = (CompanyDTO)existingCompany
            }
        };

        return Ok(response);
    }

    [HttpDelete("id")]
    public IActionResult DeleteCompany(string id) {

        var comp=_context.Company.Find(id);
        if(comp == null){
            return NotFound();
        }

        var relatedRealEstates = _context.RealEstates.Where(re => re.CompanyId == comp.Id);

        _context.RealEstates.RemoveRange(relatedRealEstates);
        _context.Company.Remove(comp);

        _context.SaveChanges();

        return NoContent();
    }
}