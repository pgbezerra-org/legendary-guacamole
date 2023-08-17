using webserver.Data;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace webserver.Pages
{

    [Authorize (Roles =Common.BZE_Role)]
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = Common.BZE_Role)]
    public class Authorized : PageModel
    {

        public Authorized()
        {
            
        }
/*
        public IActionResult OnGet()
        {
            Console.WriteLine("debug is on the console");
            Debug.WriteLine("debug is on de table");

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Accounts/Login");
            }

            return Page();
        }*/
        public IActionResult OnGet(){
            Console.WriteLine("COMECANDO A PUTARIA.");/*
            foreach(Claim cla in User.Claims){
                Console.WriteLine(cla.ToString());
            }
            Console.WriteLine("Count: "+User.Claims.Count());
            string xxx="http://schemas.microsoft.com/ws/2008/06/identity/claims/role: BZE_Role";
            Console.WriteLine(User.HasClaim(xxx, Common.BZE_Role));*/

            IServiceProvider provider = HttpContext.RequestServices;
            var cookie = Request.Cookies[Common.BZE_Cookie];
            if (cookie != null)
            {
                var dataProtector = provider.GetService<IDataProtectionProvider>().CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", CookieAuthenticationDefaults.AuthenticationScheme, "v2");
                var ticketDataFormat = new TicketDataFormat(dataProtector);
                var ticket = ticketDataFormat.Unprotect(cookie);
                var identity = ticket.Principal.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    foreach (var claim in identity.Claims)
                    {
                        Console.WriteLine($"Claim type: {claim.Type} --- Claim value: {claim.Value}");
                    }
                    Console.WriteLine(1);
                }
                Console.WriteLine(2);
            }
            Console.WriteLine(3+"\n\n\n");


            return Page();
        }
    }
}