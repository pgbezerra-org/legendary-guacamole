using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using webserver.Data;
using webserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace webserver.Pages.Account {

    public class RegisterModel : PageModel {

        [BindProperty]
        public RegisterInputModel RegisterInput { get; set; } = new RegisterInputModel();

        private readonly UserManager<Company> _userManager;
        private readonly SignInManager<Company> _signInManager;

        public RegisterModel(UserManager<Company> userManager, SignInManager<Company> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public void OnGet() {
            // Initialization or any other logic when the page is loaded
        }

        //public IActionResult OnPost() {
        public async Task<IActionResult> OnPostAsync() {

            /*if (!ModelState.IsValid) {
                return Page();
            }*/

            // Create a new user based on the registration input
            var company = new Company {
                UserName = RegisterInput.Name,
                Email = RegisterInput.Email,
                PhoneNumber=RegisterInput.PhoneNumber,
                City = RegisterInput.City,
                State = RegisterInput.State,
                Country = RegisterInput.Country
            };

            var createResult = await _userManager.CreateAsync(company, RegisterInput.Password);

            if (!createResult.Succeeded) {

                foreach (var error in createResult.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToPage("/Privacy");
            }

            var signInResult = await _signInManager.PasswordSignInAsync(company, RegisterInput.Password, isPersistent: true, lockoutOnFailure: false);

            if (!signInResult.Succeeded) {
                ModelState.AddModelError(string.Empty, "Invalid login attempt. " + createResult.ToString());
            }

            var claims = new List<Claim> {
                    new Claim(RegisterInput.Name, company.UserName),
                    new Claim(RegisterInput.Name, company.Email)
                };

            var claimsIdentity = new ClaimsIdentity(claims, LoginModel.loginCookie);
            var authProperties = new AuthenticationProperties {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.UtcNow
            };

            await HttpContext.SignInAsync(LoginModel.loginCookie, new ClaimsPrincipal(claimsIdentity), authProperties);

            // Redirect to the desired page after successful login
            return RedirectToPage("/Privacy");
        }


        public class RegisterInputModel {

            [Required]
            public string Name { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required]
            [Phone]
            public string PhoneNumber { get; set; } = string.Empty;

            [Required]
            public string City { get; set; } = string.Empty;

            [Required]
            public string State { get; set; } = string.Empty;

            [Required]
            public string Country { get; set; } = string.Empty;
        }
    }
}