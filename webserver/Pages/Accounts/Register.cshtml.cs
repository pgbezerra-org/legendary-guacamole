using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webserver.Models;

namespace webserver.Pages.Account {

    public class RegisterModel : PageModel {

        [BindProperty]
        public RegisterInputModel RegisterInput { get; set; } = new RegisterInputModel();

        private readonly UserManager<BZEmployee> _userManager;

        public RegisterModel(UserManager<BZEmployee> userManager) {
            _userManager = userManager;
        }

        public void OnGet() {
        }

        public async Task<IActionResult> OnPostAsync() {

            /*if (!ModelState.IsValid) {
                return Page();
            }*/

            var user = new BZEmployee {
                UserName = RegisterInput.Name,
                Email = RegisterInput.Email,
                PhoneNumber = RegisterInput.Phone,
                
            };

            var result = await _userManager.CreateAsync(user, RegisterInput.Password);

            if (result.Succeeded) {
                
                Console.Write("Implementa o automatic login aqui");
                Console.WriteLine("User SignUp successfull");

                return RedirectToPage("/Success");

            } else {

                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                Console.WriteLine("SignUp fail");
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
            [Phone] public string Phone { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;

        }
    }
}