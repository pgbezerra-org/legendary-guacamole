using Microsoft.AspNetCore.Mvc;
using webserver.Data;
using webserver.Models;

namespace webserver.Controllers;

[Route("clients")]
[ApiController]
public class ClientController : ControllerBase {
    
    private readonly WebserverContext _context;

    public ClientController(WebserverContext context) {
        _context = context;
    }

    class ClientSummary{
        public string Occupation { get; set; } = string.Empty;
        public string lastLogin { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    [HttpGet]
    public IActionResult ReadClients() {

        var clients = _context.Clients.Select(client => new ClientSummary {
            Occupation=client.Occupation,
            lastLogin=client.LastLogin.ToString(),  //:D
            UserName=client.UserName!,
            Email=client.Email!,
            PhoneNumber=client.PhoneNumber!
        }).ToList();

        if(clients!=null){
            return Ok(clients);
        }else{
            return NotFound();
        }
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateClient(int id, Client upClient){
        var client=_context.Clients.Find(id);
        if(client==null){
            return NotFound();
        }
        
        client.Email=upClient.Email;
        client.PhoneNumber=upClient.PhoneNumber;
        client.Occupation=upClient.Occupation;

        _context.SaveChanges();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteClient(int id){
        var client=_context.Clients.Find(id);
        if(client==null){
            return NotFound();
        }
        var result=_context.Clients.Remove(client);
        if(result == null){
            return NotFound();
        }
        _context.SaveChanges();
        return Ok();
    }
}