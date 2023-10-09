using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webserver.Models;
using webserver.Data;

namespace webserver.Pages.Views;
[Authorize]
public class ClientProfile : PageModel {

    public WebserverContext _context;
    public Client? client;

    public ClientProfile(WebserverContext context) {
        _context = context;        
    }

    public void OnGet(string id) {
        client = _context.Clients.Find(id);

        if(client==null){
            Console.WriteLine("ERROR 404");
            return;
        }

        if(client.PhoneNumber==null || client.PhoneNumber==""){
            client.PhoneNumber="Unavailable";
        }
    }
}