using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webserver.Data;
using webserver.Models;

namespace webserver.Pages.Account {

    public class RegisterModel : PageModel {

        [BindProperty]
        public RegisterInputModel RegisterInput { get; set; } = new RegisterInputModel();

        private readonly UserManager<BZEmployee> _userManager;
        private readonly SignInManager<BZEmployee> _signInManager;
        //private readonly ILogger<RegisterModel> _logger;
        //private readonly WebserverContext _context;

        public RegisterModel(UserManager<BZEmployee> userManager, SignInManager<BZEmployee> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
            //_logger = logger;
            //_context = context;
        }

        public void OnGet() {
        }

        public async Task<IActionResult> OnPostAsync() {
            /*if (!ModelState.IsValid) {
                return Page();
            }*/
/*
            if (RegisterInput.Password != RegisterInput.ConfirmPassword) {
                ModelState.AddModelError(string.Empty, "The password and confirmation password do not match.");
                return Page();
            }*/

            // Create a new user based on the registration input
            var user = new BZEmployee {
                UserName = RegisterInput.Name,
                Email = RegisterInput.Email,
                PhoneNumber = RegisterInput.Phone,
                
            };

            //var passwordHasher = new PasswordHasher<BZEmployee>();

            //var salt = Guid.NewGuid().ToString();
            //var hashedPassword = passwordHasher.HashPassword(user, salt + RegisterInput.Password);

            var result = await _userManager.CreateAsync(user, RegisterInput.Password);

            if (result.Succeeded) {
                // Optionally, you can sign in the user after successful registration
                Console.Write("Implementa o automatic login aqui");
                //await _signInManager.SignInAsync(user, isPersistent: false);

                // Redirect to a success page or the desired destination
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