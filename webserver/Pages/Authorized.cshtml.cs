using webserver.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
//THIS PAGE AND ITS CODE ARE PURELY FOR PROOF-OF-CONCEPT
namespace webserver.Pages;
[Authorize (Roles =Common.BZE_Role)]
public class Authorized : PageModel {
    
    public IActionResult OnGet() {

        IServiceProvider provider = HttpContext.RequestServices;
        var cookie = Request.Cookies[Common.BZE_Cookie];
        if (cookie != null) {

            var dataProtectionProvider = provider.GetService<IDataProtectionProvider>();
            if (dataProtectionProvider == null) {
                return RedirectToPage("/Login");
            }

            var dataProtector = dataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", CookieAuthenticationDefaults.AuthenticationScheme, "v2");
            var ticketDataFormat = new TicketDataFormat(dataProtector);
            var ticket = ticketDataFormat.Unprotect(cookie);

            if (ticket != null) {
                var identity = ticket.Principal.Identity as ClaimsIdentity;
                if (identity != null) {
                    foreach (var claim in identity.Claims) {
                        Console.WriteLine($"Claim type: {claim.Type} --- Claim value: {claim.Value}");
                    }
                }
            }
        }

        return Page();
    }
}