using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using webserver.Models;

namespace webserver.Pages.Manage {

    //[Authorize(Policy =Common.BZELevelPolicy)]
    public class Employee : PageModel {

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        private readonly UserManager<Company> _userManager;

        public Employee(UserManager<Company> userManager)
        {
            _userManager = userManager;
        }

        public class InputModel
        {
            [Required]
            [Display(Name = "Name")]
            public string Name { get; set; }=string.Empty;

            [Required]
            [Display(Name = "Email")]
            public string Email { get; set; }=string.Empty;

            [Required]
            [Display(Name = "City")]
            public string City { get; set; }=string.Empty;

            [Required]
            [Display(Name = "State")]
            public string State { get; set; }=string.Empty;
            
            [Required]
            [Display(Name = "Country")]
            public string Country { get; set; }=string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }=string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "ConfirmPassword")]
            public string ConfirmPassword { get; set; }=string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            /*
            if (ModelState.IsValid){
                return;
            }
            */

            var auxUser = await _userManager.FindByEmailAsync(Input.Email);

            if (_userManager == null) {
                ModelState.AddModelError(string.Empty, "manager not registered!");
                return Page();
            }            
            if(auxUser!=null){
                ModelState.AddModelError(string.Empty, "Email already registered!");
                return Page();
            }

            var company = new Company { UserName = Input.Name, Email = Input.Email, City = Input.City, Country = Input.Country, State = Input.State };

            var result = await _userManager.CreateAsync(company, Input.Password);

            if (result.Succeeded) {
                return RedirectToPage("/Success");
            } else {

                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                Console.WriteLine("SignUp fail");

                return Page();
            }
        }
    }
}