using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using webserver.Models;
using webserver.Data;

namespace webserver.Pages.Manage {

    [Authorize(Roles=Common.BZE_Role+","+Common.Company_Role)]
    public class ClientCreation : PageModel {

        [BindProperty]
        public ClientINFO Input { get; set; } = new ClientINFO();

        private readonly UserManager<Client> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ClientCreation(UserManager<Client> userManager, RoleManager<IdentityRole> roleManager) {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> OnPostAsync() {
            /*
            if (ModelState.IsValid){
                return;
            }
            */            

            if (_userManager == null) {
                ModelState.AddModelError(string.Empty, "manager not registered!");
                return Page();
            }            

            var auxUser = await _userManager.FindByEmailAsync(Input.Email);
            if(auxUser != null){
                ModelState.AddModelError(string.Empty, "Email already registered!");
                return Page();
            }

            var client = new Client { UserName = Input.Name, Email = Input.Email, PhoneNumber = Input.PhoneNumber,  Occupation = Input.Occupation };

            var result = await _userManager.CreateAsync(client, Input.Password);

            if (result.Succeeded) {

                var roleExist = await _roleManager.RoleExistsAsync(Common.Client_Role);
                if (!roleExist){
                    await _roleManager.CreateAsync(new IdentityRole(Common.Client_Role));
                }

                await _userManager.AddToRoleAsync(client, Common.Client_Role);
                return RedirectToPage("/Success");
            } else {

                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                Console.WriteLine("SignUp fail");

                return Page();
            }
        }

        public class ClientINFO {
            [Required]
            [Display(Name = "Name")]
            public string Name { get; set; }=string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }=string.Empty;

            [Required]
            [Display(Name = "PhoneNumber")]
            [Phone]
            public string PhoneNumber { get; set; }=string.Empty;

            [Required]
            [Display(Name = "Occupation")]
            public string Occupation { get; set; }=string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }=string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "ConfirmPassword")]
            public string ConfirmPassword { get; set; }=string.Empty;
        }
    }
}