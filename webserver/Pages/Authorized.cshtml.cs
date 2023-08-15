using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using webserver.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace webserver.Pages
{

    [Authorize(Roles = Common.BZE_Role)]
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Common.BZE_Role)]
    public class Authorized : PageModel
    {

        public Authorized()
        {
            
        }

        public IActionResult OnGet()
        {
            Console.WriteLine("debug is on the console");
            Debug.WriteLine("debug is on de table");

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Accounts/Login");
            }

            return Page();
        }
    }
}