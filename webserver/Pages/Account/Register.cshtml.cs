using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using webserver.Data;
using webserver.Models;

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
            var user = new BZEmployee {
                UserName = RegisterInput.Name,
                Email = RegisterInput.Email,
                PhoneNumber=RegisterInput.PhoneNumber,
                City = RegisterInput.City,
                State = RegisterInput.State,
                Country = RegisterInput.Country
            };

            var result = await _userManager.CreateAsync(company, RegisterInput.Password);

            if (result.Succeeded) {
                // Optionally, you can sign in the user after successful registration
                await _signInManager.SignInAsync(company, isPersistent: false);

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
            public string City { get; set; } = string.Empty;

            [Required]
            public string State { get; set; } = string.Empty;

            [Required]
            public string Country { get; set; } = string.Empty;
        }
    }
}