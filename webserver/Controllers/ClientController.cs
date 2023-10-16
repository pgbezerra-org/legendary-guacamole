using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using webserver.Data;
using webserver.Models;
using webserver.Models.DTOs;
using Newtonsoft.Json;

namespace webserver.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/client")]
public class ClientController : ControllerBase {

    private readonly WebserverContext _context;
    private readonly UserManager<Client> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ClientController(WebserverContext context, UserManager<Client> userManager, RoleManager<IdentityRole> roleManager){
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("unique/{id}")]
    public IActionResult ReadClient(string id) {

        var client = _context.Clients.Find(id);
        if(client==null){
            return NotFound();
        }

        var response = JsonConvert.SerializeObject((ClientDTO)client);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> ReadClients(string? username, int? offset, int? limit, string? sort) {

        if(limit<1){
            return BadRequest("Results amount are limited to less than 1");
        }

        var clients = _context.Clients.AsQueryable();

        if(!string.IsNullOrEmpty(username)){
            clients = clients.Where(client => client.UserName!.Contains(username));
        }


        if(!string.IsNullOrEmpty(sort)){
            sort = sort.ToLower();
            switch (sort) {
                case "name":
                    clients = clients.OrderBy(re => re.UserName);
                    break;
            }
        }

        if(offset.HasValue){
            clients = clients.Skip(offset.Value);
        }
        if(limit.HasValue){
            clients = clients.Take(limit.Value);
        }

        var resultArray = await clients.ToArrayAsync();
        var resultDtoArray = resultArray.Select(c=>(ClientDTO)c).ToArray();
        
        if(resultDtoArray==null || resultDtoArray.Length==0){
            return NotFound();
        }
        
        var response = JsonConvert.SerializeObject(resultDtoArray);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] ClientDTO clientDto, string password) {

        // Check if the email is already registered
        var existingEmail = await _userManager.FindByEmailAsync(clientDto.Email);
        if (existingEmail != null) {
            return BadRequest("Email already registered!");
        }

        var existingUsername = await _userManager.FindByNameAsync(clientDto.UserName);
        if (existingUsername != null) {
            return BadRequest("UserName already registered!");
        }

        Client client = new Client {
            Occupation = clientDto.Occupation,
            UserName = clientDto.UserName,
            Email = clientDto.Email,
            PhoneNumber = clientDto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(client, password);

        if (result.Succeeded) {

            var roleExists = await _roleManager.RoleExistsAsync(Common.Client_Role);
            if (!roleExists) {
                await _roleManager.CreateAsync(new IdentityRole(Common.Client_Role));
            }

            await _userManager.AddToRoleAsync(client, Common.Client_Role);

            return CreatedAtAction(nameof(CreateClient), (ClientDTO)client);
        }else{
            return StatusCode(500, "Internal Server Error: Register Client Unsuccessful");
        }
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateClient([FromBody] ClientDTO newClient) {

        var existingClient = _context.Clients.Find(newClient.Id);
        if (existingClient==null) {
            return BadRequest("Client does not Exist!");
        }

        existingClient.UserName = newClient.UserName;
        existingClient.PhoneNumber = newClient.PhoneNumber;
        
        existingClient.Occupation = newClient.Occupation;

        await _context.SaveChangesAsync();

        var response = JsonConvert.SerializeObject((ClientDTO)existingClient);

        return Ok(response);
    }

    [HttpDelete("id")]
    public async Task<IActionResult> DeleteClient(string id) {

        var client = _context.Clients.Find(id);
        if(client == null){
            return BadRequest("Client does not Exist!");
        }
        
        _context.Clients.Remove(client);

        await _context.SaveChangesAsync();

        return NoContent();
    }    
}