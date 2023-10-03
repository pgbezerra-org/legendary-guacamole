using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using webserver.Data;
using webserver.Models;
using webserver.Models.DTOs;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace webserver.Controllers;
[Authorize(Roles=Common.BZE_Role+","+Common.Company_Role)]
[ApiController]
[Route("api/v1/company")]
public class BZEmployeeController : ControllerBase {

    private readonly WebserverContext _context;
    private readonly UserManager<Company> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public BZEmployeeController(WebserverContext context, UserManager<Company> userManager, RoleManager<IdentityRole> roleManager) {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("unique/{id}")]
    public async Task<IActionResult> ReadBZEmployee(string id){

    }

    [HttpPost]
    public async Task<IActionResult> CreateBZEmployee(){

    }

    [HttpPatch]
    public async Task<IActionResult> UpdateBZEmployee(){

    }

    [HttpDelete("id")]
    public async Task<IActionResult> DeleteBZEmployee(string id){

        var employee = _context.BZEmployees.Find(id);
        if(employee == null){
            return BadRequest("Colaborator does not Exist!");
        }

        _context.BZEmployees.Remove(employee);

        await _context.SaveChangesAsync();

        return NoContent();
    }        
}