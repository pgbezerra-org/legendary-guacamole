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
/// Controller class for Client CRUD requests via the HTTP API
/// Responses are sent only in JSON
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/client")]
[Produces("application/json")]
public class ClientController : ControllerBase {

    private readonly WebserverContext _context;
    private readonly UserManager<Client> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    /// <summary>
    /// Controller class for Client CRUD requests via the HTTP API
    /// Responses are sent only in JSON
    /// </summary>
    public ClientController(WebserverContext context, UserManager<Client> userManager, RoleManager<IdentityRole> roleManager){
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Get the Client with the given Id
    /// </summary>
    /// <returns>Client of the given User. NotFoundResult if there is none</returns>
    /// <response code="200">Returns the Client's DTO</response>
    /// <response code="404">If there is none with the given Id</response>
    [HttpGet("{id}")]
    public IActionResult ReadClient(string id) {

        var client = _context.Clients.Find(id);
        if(client==null){
            return NotFound();
        }

        var response = JsonConvert.SerializeObject((ClientDTO)client);

        return Ok(response);
    }

    /// <summary>
    /// Get an array of Clients DTO, with optional filters
    /// </summary>
    /// <returns>ClientDTO Array</returns>
    /// <param name="username">Filters results to only Users whose username contains this string</param>
    /// <param name="offset">Offsets the result by a given amount</param>
    /// <param name="limit">Limits the number of results</param>
    /// <param name="sort">Orders the result by a given field. Does not order if the field does not exist</param>
    /// <response code="200">Returns an array of Client DTOs</response>
    /// <response code="404">If no Clients fit the given filters</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Client>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    [HttpGet]
    public async Task<IActionResult> ReadClients(string? username, int? offset, int limit, string? sort) {

        if(limit<1){
            return BadRequest("Limit parameter must be a natural number greater than 0");
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
        clients = clients.Take(limit);

        var resultArray = await clients.ToArrayAsync();
        var resultDtoArray = resultArray.Select(c=>(ClientDTO)c).ToArray();
        
        if(resultDtoArray==null || resultDtoArray.Length==0){
            return NotFound();
        }
        
        var response = JsonConvert.SerializeObject(resultDtoArray);

        return Ok(response);
    }

    /// <summary>
    /// Creates a Client User
    /// </summary>
    /// <returns>The created Client Data</returns>
    /// <response code="200">ClientDTO</response>
    /// <response code="400">In case the Email or Username is already Registered (it will tell which)</response>
    /// <response code="500">Returns a string with the requirements in the data which weren't filled (weak password, empty fields, etc)</response>
    /// <response code="500">Returns a string with server-side error(s)</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Client))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestObjectResult))]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
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

        if(!result.Succeeded){
            return StatusCode(500, "Internal Server Error: Register Client Unsuccessful\n\n"+result.Errors);
        }

        var roleExists = await _roleManager.RoleExistsAsync(Common.Client_Role);
        if (!roleExists) {
            await _roleManager.CreateAsync(new IdentityRole(Common.Client_Role));
        }

        await _userManager.AddToRoleAsync(client, Common.Client_Role);

        return CreatedAtAction(nameof(CreateClient), (ClientDTO)client);
    }

    /// <summary>
    /// Updates the Client with the given Id and data
    /// </summary>
    /// <returns>Client's DTO with the updated Data</returns>
    /// <response code="200">ClientdDTO with the updated data</response>
    /// <response code="400">If a Client with the given Id was not found</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Client))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestObjectResult))]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Deletes the Client with the given Id
    /// </summary>
    /// <returns>NoContent if successfull</returns>
    /// <response code="200">User was found, and thus deleted</response>
    /// <response code="400">User not found</response>
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Client))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadRequestObjectResult))]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id}")]
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