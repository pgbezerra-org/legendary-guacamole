using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webserver.Models;

namespace webserver.Pages.Account {

    public class LoginModel : PageModel {

        [BindProperty]
        public CredentialInput Credential { get; set; } = new CredentialInput();

        private readonly SignInManager<BZEmployee> _signInManager;
        private readonly UserManager<BZEmployee> _userManager;

        public LoginModel(SignInManager<BZEmployee> signInManager, UserManager<BZEmployee> userManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public void OnGet() {

        }

        public async Task<IActionResult> OnPostAsync() {

            /*if (!ModelState.IsValid) {
                return Page();
            }*/

            var user = await _userManager.FindByEmailAsync(Credential.Email);

            if (_userManager == null) {
                ModelState.AddModelError(string.Empty, "manager not registered!");
                return Page();
            }            

            if (user == null) {
                ModelState.AddModelError(string.Empty, "Email not registered!");
                return Page();
            }

            //Just to stop with the compiler warnings
            // Ensure the user properties are not null before accessing them
            if (user.PasswordHash is null || user.UserName is null || user.Email is null) {
                return Page();
            }
            
            var result = await _signInManager.PasswordSignInAsync(user, Credential.Password, isPersistent: true, lockoutOnFailure: false);

            if (!result.Succeeded) {
                ModelState.AddModelError(string.Empty, "Invalid login attempt. " + result.ToString());
                return Page();
            }

            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.UtcNow
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            // Redirect to the desired page after successful login
            return RedirectToPage("/Privacy");
        }

        public class CredentialInput {

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

        }
    }
}