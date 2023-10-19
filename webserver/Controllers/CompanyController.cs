using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using webserver.Data;
using webserver.Models;
using webserver.Models.DTOs;
using webserver.Utilities;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace webserver.Controllers;

[Authorize(Roles=Common.BZE_Role)]
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

        var response = JsonConvert.SerializeObject((CompanyDTO)company);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> ReadCompanies(int? offset, int? limit, string? Country, string? State, string? City, string? sort) {

        if(limit<1){
            return BadRequest("Results amount are limited to less than 1");
        }

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

        if(!string.IsNullOrEmpty(sort)){
            sort = sort.ToLower();
            switch (sort) {
                case "city":
                    companies = companies.OrderBy(re => re.City).ThenBy(re => re.State).ThenBy(re => re.Country).ThenBy(re => re.UserName);
                    break;
                case "state":
                    companies = companies.OrderBy(re => re.State).ThenBy(re => re.City).ThenBy(re => re.Country).ThenBy(re => re.UserName);
                    break;
                case "country":
                    companies = companies.OrderBy(re => re.Country).ThenBy(re => re.State).ThenBy(re => re.City).ThenBy(re => re.UserName);
                    break;
                case "name":
                    companies = companies.OrderBy(re => re.UserName).ThenBy(re => re.Country).ThenBy(re => re.State).ThenBy(re => re.City);
                    break;
            }
        }

        if(offset.HasValue){
            companies = companies.Skip(offset.Value);
        }
        if(limit.HasValue){
            companies = companies.Take(limit.Value);
        }

        var resultArray = await companies.ToArrayAsync();
        var resultDtoArray = resultArray.Select(c=>(CompanyDTO)c).ToArray();
        
        if(resultDtoArray==null || resultDtoArray.Length==0){
            return NotFound();
        }
        
        var response = JsonConvert.SerializeObject(resultDtoArray);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyDTO companyDto, string password) {

        // Check if the email is already registered
        var existingEmail = await _userManager.FindByEmailAsync(companyDto.Email);
        if (existingEmail != null) {
            return BadRequest("Email already registered!");
        }

        var existingUsername = await _userManager.FindByNameAsync(companyDto.UserName);
        if (existingUsername != null) {
            return BadRequest("UserName already registered!");
        }

        Company company = new Company {
            City = companyDto.City,
            State = companyDto.State,
            Country = companyDto.Country,
            UserName = companyDto.UserName,
            Email = companyDto.Email,
            PhoneNumber = companyDto.PhoneNumber
        };        

        var result = await _userManager.CreateAsync(company, password);

        if (result.Succeeded) {

            var roleExists = await _roleManager.RoleExistsAsync(Common.Company_Role);
            if (!roleExists) {
                await _roleManager.CreateAsync(new IdentityRole(Common.Company_Role));
            }

            await _userManager.AddToRoleAsync(company, Common.Company_Role);

            return CreatedAtAction(nameof(CreateCompany), (CompanyDTO)company);
        }else{
            return StatusCode(500, "Internal Server Error: Register Company Unsuccessful");
        }
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateCompany([FromBody] CompanyDTO newCompany) {

        var existingCompany = _context.Company.Find(newCompany.Id);
        if (existingCompany==null) {
            return BadRequest("Company does not Exist!");
        }

        existingCompany.UserName = newCompany.UserName;
        existingCompany.PhoneNumber = newCompany.PhoneNumber;
        
        existingCompany.City = newCompany.City;
        existingCompany.State = newCompany.State;
        existingCompany.Country = newCompany.Country;

        await _context.SaveChangesAsync();

        var response = JsonConvert.SerializeObject((CompanyDTO)existingCompany);

        return Ok(response);
    }

    [HttpDelete("id")]
    public async Task<IActionResult> DeleteCompany(string id) {

        var comp = _context.Company.Find(id);
        if(comp == null){
            return BadRequest("Company does not Exist!");
        }

        var relatedRealEstates = _context.RealEstates.Where(re => re.CompanyId == comp.Id);

        _context.RealEstates.RemoveRange(relatedRealEstates);
        _context.Company.Remove(comp);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}