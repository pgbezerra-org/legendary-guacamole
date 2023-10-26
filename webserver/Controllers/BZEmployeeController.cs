using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using webserver.Data;
using webserver.Models;
using webserver.Models.DTOs;
using Newtonsoft.Json;

namespace webserver.Controllers;

/// <summary>
/// Controller class for BZEmployee CRUD requests via the HTTP API
/// Responses are sent only in JSON
/// </summary>
[Authorize(Roles=Common.BZE_Role)]
[ApiController]
[Route("api/v1/bzemployee")]
public class BZEmployeeController : ControllerBase {

    private readonly WebserverContext _context;
    private readonly UserManager<BZEmployee> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    /// <summary>
    /// BZEmployee API Controller Constructor
    /// Contains the essential for such Controller: IdentityDbController, UserManager<SpecificIdentityUser> and RoleManager<IdentityRole>
    /// </summary>
    public BZEmployeeController(WebserverContext context, UserManager<BZEmployee> userManager, RoleManager<IdentityRole> roleManager){
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Get the BZEmployee with the given Id
    /// </summary>
    /// <returns>BZEmployeeDTO of the given User. NotFoundResult if there is none</returns>
    /// <response code="200">Returns the BZEmployee's DTO</response>
    /// <response code="404">If there is none with the given Id</response>
    [HttpGet("{id}")]
    public IActionResult ReadBZEmployee(string id) {

        var bzemp = _context.BZEmployees.Find(id);
        if(bzemp==null){
            return NotFound();
        }

        var response = JsonConvert.SerializeObject((BZEmployeeDTO)bzemp);

        return Ok(response);
    }

    /// <summary>
    /// Get an array of BZEmployees DTO, with optional filters
    /// </summary>
    /// <returns>BZEmployeeDTO Array</returns>
    /// <param name="username">Filters results to only Users whose username contains this string</param>
    /// <response code="200">Returns an array of BZEmployee DTOs</response>
    /// <response code="404">If no BZEmployees fit the given filters</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BZEmployee>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [HttpGet]
    public async Task<IActionResult> ReadBZEmployees(string? username, int? offset, int limit, string? sort) {

        if(limit<1){
            return BadRequest("Limit parameter must be a natural number greater than 0");
        }

        var bzemployees = _context.BZEmployees.AsQueryable();

        if(!string.IsNullOrEmpty(username)){
            bzemployees = bzemployees.Where(bzemp => bzemp.UserName!.Contains(username));
        }


        if(!string.IsNullOrEmpty(sort)){
            sort = sort.ToLower();
            switch (sort) {
                case "name":
                    bzemployees = bzemployees.OrderBy(re => re.UserName);
                    break;
            }
        }

        if(offset.HasValue){
            bzemployees = bzemployees.Skip(offset.Value);
        }

        bzemployees = bzemployees.Take(limit);

        var resultArray = await bzemployees.ToArrayAsync();
        var resultDtoArray = resultArray.Select(c=>(BZEmployeeDTO)c).ToArray();
        
        if(resultDtoArray==null || resultDtoArray.Length==0){
            return NotFound();
        }
        
        var response = JsonConvert.SerializeObject(resultDtoArray);

        return Ok(response);
    }

    /// <summary>
    /// Creates a BZEmployee User
    /// </summary>
    /// <returns>The created BZEmployee Data</returns>
    /// <response code="200">BZEmployeeDTO</response>
    /// <response code="400">In case the Email or Username is already Registered (it will tell which)</response>
    /// <response code="500">Returns a string with the requirements in the data which weren't filled (weak password, empty fields, etc)</response>
    /// <response code="500">Returns a string with server-side error(s)</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BZEmployee))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestObjectResult))]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> CreateBZEmployee([FromBody] BZEmployeeDTO bzempDto, string password) {

        // Check if the email is already registered
        var existingEmail = await _userManager.FindByEmailAsync(bzempDto.Email);
        if (existingEmail != null) {
            return BadRequest("Email already registered!");
        }

        var existingUsername = await _userManager.FindByNameAsync(bzempDto.UserName);
        if (existingUsername != null) {
            return BadRequest("UserName already registered!");
        }

        BZEmployee bzemp = new BZEmployee {
            salary = bzempDto.Salary,
            UserName = bzempDto.UserName,
            Email = bzempDto.Email,
            PhoneNumber = bzempDto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(bzemp, password);

        if(!result.Succeeded){
            return StatusCode(500, "Internal Server Error: Register BZEmployee Unsuccessful\n\n" + result.Errors);
        }
        
        var roleExists = await _roleManager.RoleExistsAsync(Common.BZE_Role);
        if (!roleExists) {
            await _roleManager.CreateAsync(new IdentityRole(Common.BZE_Role));
        }

        await _userManager.AddToRoleAsync(bzemp, Common.BZE_Role);

        return CreatedAtAction(nameof(CreateBZEmployee), (BZEmployeeDTO)bzemp);
    }

    /// <summary>
    /// Updates the BZEmployee with the given Id and data
    /// </summary>
    /// <returns>BZEmployee's DTO with the updated Data</returns>
    /// <response code="200">BZEmployeedDTO with the updated data</response>
    /// <response code="400">If a BZEmployee with the given Id was not found</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BZEmployee))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestObjectResult))]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    [HttpPatch]
    public async Task<IActionResult> UpdateBZEmployee([FromBody] BZEmployeeDTO newEmp) {

        var existingBZEmp = _context.BZEmployees.Find(newEmp.Id);
        if (existingBZEmp==null) {
            return BadRequest("BZEmployee does not Exist!");
        }

        existingBZEmp.UserName = newEmp.UserName;
        existingBZEmp.PhoneNumber = newEmp.PhoneNumber;
        
        existingBZEmp.salary = newEmp.Salary;

        await _context.SaveChangesAsync();

        var response = JsonConvert.SerializeObject((BZEmployeeDTO)existingBZEmp);

        return Ok(response);
    }

    /// <summary>
    /// Deletes the BZEmployee with the given Id
    /// </summary>
    /// <returns>NoContent if successfull</returns>
    /// <response code="200">User was found, and thus deleted</response>
    /// <response code="400">User not found</response>
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(BZEmployee))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestObjectResult))]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    [HttpDelete("id")]
    public async Task<IActionResult> DeleteBZEmployee(string id) {

        var bzemp = _context.BZEmployees.Find(id);
        if(bzemp == null){
            return BadRequest("BZEmployee does not Exist!");
        }
        
        _context.BZEmployees.Remove(bzemp);

        await _context.SaveChangesAsync();

        return NoContent();
    }    
}