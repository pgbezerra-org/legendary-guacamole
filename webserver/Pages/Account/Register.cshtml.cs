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

            var auxUser = await _userManager.FindByEmailAsync(RegisterInput.Email);

            if (_userManager == null) {
                ModelState.AddModelError(string.Empty, "manager not registered!");
                return Page();
            }
            if(auxUser!=null){
                ModelState.AddModelError(string.Empty, "Email already registered!");
                return Page();
            }

            var user = new BZEmployee {
                UserName = RegisterInput.Name,
                Email = RegisterInput.Email,
                PhoneNumber=RegisterInput.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, RegisterInput.Password);

            if (result.Succeeded) {

                await _userManager.AddToRoleAsync(user,Common.BZE_Role);
                //Console.WriteLine("Implementa o automatic login aqui");
                return RedirectToPage("/Success");
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