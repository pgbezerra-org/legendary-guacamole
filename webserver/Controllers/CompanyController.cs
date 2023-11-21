using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using webserver.Data;
using webserver.Models;
using webserver.Models.DTOs;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace webserver.Controllers;

[Authorize(Roles=Common.BZE_Role)]
/// <summary>
/// Controller class for Company CRUD requests via the HTTP API
/// Responses are sent only in JSON
/// </summary>
[ApiController]
[Route("api/v1/company")]
[Produces("application/json")]
public class CompanyController : ControllerBase {
    
    private readonly WebserverContext _context;
    private readonly UserManager<Company> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    /// <summary>
    /// Company API Controller Constructor
    /// Contains the essential for such Controller: IdentityDbController, UserManager TSpecificIdentityUser and RoleManager TIdentityRole
    /// </summary>
    public CompanyController(WebserverContext context, UserManager<Company> userManager, RoleManager<IdentityRole> roleManager) {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("unique/{id}")]
    public IActionResult ReadCompany(string id) {
    /// <summary>
    /// Get the Company with the given Id
    /// </summary>
    /// <returns>Company of the given User. NotFoundResult if there is none</returns>
    /// <response code="200">Returns the Company's DTO</response>
    /// <response code="404">If there is none with the given Id</response>
        var company = _context.Company.Find(id);
        if(company==null){
            return NotFound();
        }

        var response = JsonConvert.SerializeObject((CompanyDTO)company);

        return Ok(response);
    }

    /// <summary>
    /// Get an array of Companies DTO, with optional filters
    /// </summary>
    /// <returns>CompanyDTO Array</returns>
    /// <param name="Country">Filters the Companies by Country of origin</param>
    /// <param name="State">Filters the Companies by State of origin</param>
    /// <param name="City">Filters the Companies by City of origin</param>
    /// <param name="offset">Offsets the result by a given amount</param>
    /// <param name="limit">Limits the number of results</param>
    /// <param name="sort">Orders the result by a given field. Does not order if the field does not exist</param>
    /// <response code="200">Returns an array of Company DTOs</response>
    /// <response code="404">If no Companies fit the given filters</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Company>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [HttpGet]
    public async Task<IActionResult> ReadCompanies(int? offset, int limit, string? Country, string? State, string? City, string? sort) {

        if(limit<1){
            return BadRequest("Limit parameter must be a natural number greater than 0");
        }

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
        companies = companies.Take(limit);

        var resultArray = await companies.ToArrayAsync();
        var resultDtoArray = resultArray.Select(c=>(CompanyDTO)c).ToArray();
        
        if(resultDtoArray==null || resultDtoArray.Length==0){
            return NotFound();
        }
        
        var response = JsonConvert.SerializeObject(resultDtoArray);

        return Ok(response);
    }

    /// <summary>
    /// Creates a Company User
    /// </summary>
    /// <returns>The created Company Data</returns>
    /// <response code="200">CompanyDTO</response>
    /// <response code="400">In case the Email or Username is already Registered (it will tell which)</response>
    /// <response code="500">Returns a string with the requirements in the data which weren't filled (weak password, empty fields, etc)</response>
    /// <response code="500">Returns a string with server-side error(s)</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Company))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestObjectResult))]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Updates the Company with the given Id and data
    /// </summary>
    /// <returns>Company's DTO with the updated Data</returns>
    /// <response code="200">CompanydDTO with the updated data</response>
    /// <response code="400">If a Company with the given Id was not found</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Company))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestObjectResult))]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Deletes the Company with the given Id
    /// </summary>
    /// <returns>NoContent if successfull</returns>
    /// <response code="200">User was found, and thus deleted</response>
    /// <response code="400">User not found</response>
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Company))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestObjectResult))]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
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