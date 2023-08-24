using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using webserver.Data;
using webserver.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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
                PhoneNumber=RegisterInput.PhoneNumber,
                City = RegisterInput.City,
                State = RegisterInput.State,
                Country = RegisterInput.Country
            };

            var result = await _userManager.CreateAsync(company, RegisterInput.Password);

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
            public string City { get; set; } = string.Empty;

            [Required]
            public string State { get; set; } = string.Empty;

            [Required]
            public string Country { get; set; } = string.Empty;
        }
    }
}