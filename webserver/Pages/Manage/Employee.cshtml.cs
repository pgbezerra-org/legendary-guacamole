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

        private readonly UserManager<BZEmployee> _userManager;

        public Employee(UserManager<BZEmployee> userManager)
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
                var user = new BZEmployee { UserName = Input.Email, Email = Input.Email };
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