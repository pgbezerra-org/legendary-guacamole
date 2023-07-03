using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webserver.Models;

namespace webserver.Pages.Account {

    public class LoginModel : PageModel {

        [BindProperty]
        public Credential credential { get; set; } = new Credential();
        
        public readonly static string loginCookie = "BZEmploy";

        private readonly SignInManager<Company> _signInManager;
        private readonly UserManager<Company> _userManager;

        public LoginModel(SignInManager<Company> signInManager, UserManager<Company> userManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public void OnGet() {

        }

        public async Task<IActionResult> OnPostAsync() {

            /*if (!ModelState.IsValid) {
                return Page();
            }*/

            var user = await _userManager.FindByEmailAsync(credential.Email);

            if (user == null) {
                ModelState.AddModelError(string.Empty, "User not registered!");
                return Page();
            }

            //Just to stop with the compiler warnings
            // Ensure the user properties are not null before accessing them
            if (user.PasswordHash is null || user.UserName is null || user.Email is null) {
                // Handle the case where one or more properties are null
                return Page();
            }

            var passwordHasher = new PasswordHasher<Company>();
            var passwordResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, credential.Password);

            if (passwordResult != PasswordVerificationResult.Success) {
                ModelState.AddModelError(string.Empty, "Wrong Email or Password");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, credential.Password, isPersistent: true, lockoutOnFailure: false);

            if (!result.Succeeded) {
                ModelState.AddModelError(string.Empty, "Invalid login attempt. " + result.ToString());
                return Page();
            }

            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };

            var claimsIdentity = new ClaimsIdentity(claims, loginCookie);
            var authProperties = new AuthenticationProperties {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.UtcNow
            };

            await HttpContext.SignInAsync(loginCookie, new ClaimsPrincipal(claimsIdentity), authProperties);

            // Redirect to the desired page after successful login
            return RedirectToPage("/Privacy");
        }

        public class Credential {

            [System.ComponentModel.DataAnnotations.Required]
            public string UserName { get; set; } = string.Empty;

            [System.ComponentModel.DataAnnotations.Required]
            public string Email { get; set; } = string.Empty;

            [System.ComponentModel.DataAnnotations.Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

        }
    }
}