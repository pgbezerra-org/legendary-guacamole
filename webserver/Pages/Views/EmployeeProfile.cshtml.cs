using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using webserver.Models;
using webserver.Data;

namespace webserver.Pages.Views;
[Authorize(Roles =Common.BZE_Role)]
public class EmployeeProfile : PageModel {

    public WebserverContext context;
    public BZEmployee? employee;

    public EmployeeProfile(WebserverContext _context) {
        context = _context;
    }

    public void OnGet() {
    }
}