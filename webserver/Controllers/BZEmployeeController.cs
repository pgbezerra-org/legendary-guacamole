using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using webserver.Data;
using webserver.Models;
using webserver.Utilities;
using webserver.Models.DTOs;
using Newtonsoft.Json;

namespace webserver.Controllers;

[Authorize(Roles=Common.BZE_Role)]
[ApiController]
[Route("api/v1/bzemployee")]
public class BZEmployeeController : ControllerBase {

    private readonly WebserverContext _context;
    private readonly UserManager<BZEmployee> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public BZEmployeeController(WebserverContext context, UserManager<BZEmployee> userManager, RoleManager<IdentityRole> roleManager){
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("unique/{id}")]
    public IActionResult ReadBZEmployee(string id) {

        var bzemp = _context.BZEmployees.Find(id);
        if(bzemp==null){
            return NotFound();
        }

        var response = JsonConvert.SerializeObject((BZEmployeeDTO)bzemp);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> ReadBZEmployees(string? username, int? offset, int? limit, string? sort) {

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
        if(limit.HasValue){
            bzemployees = bzemployees.Take(limit.Value);
        }

        var resultArray = await bzemployees.ToArrayAsync();
        var resultDtoArray = resultArray.Select(c=>(BZEmployeeDTO)c).ToArray();
        
        if(resultDtoArray==null || resultDtoArray.Length==0){
            return NotFound();
        }
        
        var response = JsonConvert.SerializeObject(resultDtoArray);

        return Ok(response);
    }

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
            return StatusCode(500, "Internal Server Error: Register Client Unsuccessful\n\n"+result.Errors);
        }

        var roleExists = await _roleManager.RoleExistsAsync(Common.BZE_Role);
        if (!roleExists) {
            await _roleManager.CreateAsync(new IdentityRole(Common.BZE_Role));
        }

        await _userManager.AddToRoleAsync(bzemp, Common.BZE_Role);

        return CreatedAtAction(nameof(CreateBZEmployee), (BZEmployeeDTO)bzemp);
    }

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