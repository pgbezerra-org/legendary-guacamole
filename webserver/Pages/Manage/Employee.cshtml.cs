using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace webserver.Pages.Manage {

    public class Employee : PageModel {

        [BindProperty]
        public InputModel Input { get; set; }

        private readonly UserManager<IdentityUser> _userManager;

        public Employee(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public class InputModel
        {
            [Required]
            [Display(Name = "Email")]
            public string Email { get; set; }=string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }=string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    // User created successfully
                }
                else
                {
                    // Error creating user
                }
            }
            return Page();
        }
    }
}