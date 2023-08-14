using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.Extensions;
using webserver.Models;
using webserver.Data;
using webserver;

namespace webserver.Pages.Account {

    public class RegisterModel : PageModel {

        [BindProperty]
        public RegisterInputModel RegisterInput { get; set; } = new RegisterInputModel();

        private readonly UserManager<BZEmployee> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(UserManager<BZEmployee> userManager, RoleManager<IdentityRole> roleManager) {
            _userManager = userManager;
            _roleManager=roleManager;
        }

        public void OnGet() {
        }

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
                PhoneNumber = RegisterInput.Phone                
            };

            //var role1 = await roleManager.FindByNameAsync("Employee");
            /*
            var role = new IdentityRole(Common.BZERole);
            await _roleManager.CreateAsync(role);
            await _roleManager.AddClaimAsync(role, new Claim(ClaimTypes.Role, Common.BZERole));
            */
            var result = await _userManager.CreateAsync(user, RegisterInput.Password);

            if (result.Succeeded) {

                await _userManager.AddToRoleAsync(user,Common.BZERole);
                _roleManager.add
                
                Console.WriteLine("Implementa o automatic login aqui");
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