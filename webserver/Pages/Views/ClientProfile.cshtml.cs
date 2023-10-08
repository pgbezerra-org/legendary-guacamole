using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
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

    public void OnGet() {
    }
}