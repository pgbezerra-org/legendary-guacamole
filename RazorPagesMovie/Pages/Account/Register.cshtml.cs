using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Authentication;

namespace RazorPagesMovie.Pages.Account {

    public class RegisterModel : PageModel {

        [BindProperty]
        public RegisterInputModel RegisterInput { get; set; } = new RegisterInputModel();

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RazorPagesMovieContext _context;

        public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RazorPagesMovieContext context, ILogger<RegisterModel> logger) {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        public void OnGet() {
        }

        public async Task<IActionResult> OnPostAsync() {
            /*if (!ModelState.IsValid) {
                return Page();
            }*/

            // Create a new user based on the registration input
            var user = new Company {
                UserName = RegisterInput.Name,
                Email = RegisterInput.Email,
                PhoneNumber = RegisterInput.Phone,
                City = RegisterInput.City,
                State = RegisterInput.State,
                Country = RegisterInput.Country
            };

            var passwordHasher = new PasswordHasher<Company>();

            var salt = Guid.NewGuid().ToString();
            var hashedPassword = passwordHasher.HashPassword(user, salt + RegisterInput.Password);

            user.Salt = salt;
            user.PasswordHash = hashedPassword;

            _context.Company.Add(user);

            var result = await _userManager.CreateAsync(user, RegisterInput.Password);

            if (result.Succeeded) {
                // Optionally, you can sign in the user after successful registration
                await _signInManager.SignInAsync(user, isPersistent: false);

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

            [Required]
            public string City { get; set; } = string.Empty;

            [Required]
            public string State { get; set; } = string.Empty;

            [Required]
            public string Country { get; set; } = string.Empty;
        }
    }
}

