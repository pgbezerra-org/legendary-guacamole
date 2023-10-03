using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using webserver.Data;
using webserver.Models;
using webserver.Models.DTOs;
using Newtonsoft.Json;

namespace webserver.Controllers;

[Authorize(Roles=Common.BZE_Role+","+Common.Company_Role)]
[ApiController]
[Route("api/v1/company")]
public class ClientController : ControllerBase {

    private readonly WebserverContext _context;
    private readonly UserManager<Company> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ClientController(WebserverContext context, UserManager<Company> userManager, RoleManager<IdentityRole> roleManager){
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("unique/{id}")]
    public IActionResult ReadCompany(string id) {

        var client = _context.Clients.Find(id);
        if(client==null){
            return NotFound();
        }

        var response = JsonConvert.SerializeObject((Client)client);

        return Ok(response);
    }

    [HttpDelete("id")]
    public async Task<IActionResult> DeleteCompany(string id) {

        var client = _context.Clients.Find(id);
        if(client == null){
            return BadRequest("Client does not Exist!");
        }
        
        _context.Clients.Remove(client);

        await _context.SaveChangesAsync();

        return NoContent();
    }
    
}