using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using webserver.Models;
using webserver.Data;

namespace webserver.Pages.Views;
[Authorize(Roles=Common.BZE_Role)]
public class EmployeeProfile : PageModel {

    public WebserverContext _context;
    public BZEmployee? employee;

    public EmployeeProfile(WebserverContext context) {
        _context = context;
    }

    public void OnGet(string id) {
        employee = _context.BZEmployees.Find(id);

        if(employee==null){
            Console.WriteLine("ERROR 404");
            return;
        }

        if(employee.PhoneNumber==null || employee.PhoneNumber==""){
            employee.PhoneNumber="Unavailable";
        }
    }
}