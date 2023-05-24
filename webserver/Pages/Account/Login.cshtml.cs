using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using webserver.Models;

namespace webserver.Pages.Account {

    public class LoginModel : PageModel {

        [BindProperty]
        public Credential Credential { get; set; } = new Credential();

        private readonly SignInManager<Company> _signInManager;

        public LoginModel(SignInManager<Company> signInManager) {
            _signInManager = signInManager;
        }

        public void OnGet() {
            
        }

        public async Task<IActionResult> OnPostAsync() {

            /*if (!ModelState.IsValid) {
                return Page();
            }*/

            var result = await _signInManager.PasswordSignInAsync(Credential.Email, Credential.Password, isPersistent: true, lockoutOnFailure: false);

            if (!result.Succeeded) {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com")
                };

            var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
            var authProperties = new AuthenticationProperties {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = false,
                IssuedUtc = DateTimeOffset.UtcNow
            };

            await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

            // Redirect to the desired page after successful login
            return RedirectToPage("/Privacy");
        }
    }

    public class Credential {

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

    }
}