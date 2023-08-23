using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace webserver.Pages.Accounts
{
    public class LogOut : PageModel
    {
        private readonly ILogger<LogOut> _logger;

        public LogOut(ILogger<LogOut> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string logout)
    {
        if (logout == "Yes")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Accounts/Login");
        }
        else
        {
            return RedirectToPage("/Index");
        }
    }
    }
}