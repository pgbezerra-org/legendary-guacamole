using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using webserver.Models;

namespace webserver.Pages.Manage;

[Authorize(Roles=Common.BZE_Role)]
public class CompanyCreation : PageModel {

    [BindProperty]
    public CompanyINFO Input { get; set; } = new CompanyINFO();

    private readonly UserManager<Company> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public CompanyCreation(UserManager<Company> userManager, RoleManager<IdentityRole> roleManager) {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> OnPostAsync() {
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
        if(auxUser != null){
            ModelState.AddModelError(string.Empty, "Email already registered!");
            return Page();
        }

        //The Id field will be overwritten by the _userManager at the Element's creation
        var company = new Company("whatever",Input.Name,Input.Email,Input.PhoneNumber,Input.Country,Input.State,Input.City);

        var result = await _userManager.CreateAsync(company, Input.Password);

        if (result.Succeeded) {

            var roleExist = await _roleManager.RoleExistsAsync(Common.Company_Role);
            if (!roleExist){
                await _roleManager.CreateAsync(new IdentityRole(Common.Company_Role));
            }

            await _userManager.AddToRoleAsync(company, Common.Company_Role);
            return RedirectToPage("/Success");
        } else {

            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            Console.WriteLine("SignUp fail");

            return Page();
        }
    }

    public class CompanyINFO {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name ="PhoneNumber")]
        public string PhoneNumber {get; set;} = string.Empty;

        [Required]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Required]
        [Display(Name = "State")]
        public string State { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}