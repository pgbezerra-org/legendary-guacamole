using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using webserver.Data;
using webserver.Models;

namespace webserver.Pages.Account {
namespace webserver.Pages.Account {

    public class RegisterModel : PageModel {

        [BindProperty]
        public RegisterInputModel registerInput { get; set; } = new RegisterInputModel();

        private readonly UserManager<BZEmployee> _userManager;
        private readonly SignInManager<BZEmployee> _signInManager;

        public RegisterModel(UserManager<BZEmployee> userManager, SignInManager<BZEmployee> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //public IActionResult OnPost() {
        public async Task<IActionResult> OnPostAsync() {


            /*if (!ModelState.IsValid) {
                return Page();
            }*/

            var company = new BZEmployee {
                UserName = RegisterInput.Name,
                Email = RegisterInput.Email,
                PhoneNumber=RegisterInput.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, RegisterInput.Password);

            if (result.Succeeded) {
                // Optionally, you can sign in the user after successful registration
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Redirect to a success page or the desired destination
                return RedirectToPage("/Privacy");
            } else {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }
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