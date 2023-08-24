using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using webserver.Models;
using MySqlConnector;

namespace webserver.Pages.Account {

    public class LoginModel : PageModel {

        [BindProperty]
        public Credential Credential { get; set; } = new Credential();

        private readonly SignInManager<Company> _signInManager;
        private readonly UserManager<Company> _userManager;

        public LoginModel(SignInManager<Company> signInManager, UserManager<Company> userManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public void OnGet() {
            
        }

        public async Task<IActionResult> OnPostAsync() {

            /*if (!ModelState.IsValid) {
                return Page();
            }*/

            var user = await _userManager.FindByEmailAsync(Credential.Email);

            if (user == null) {
                ModelState.AddModelError(string.Empty, "User not registered!");
                return Page();
            }

            //Just to stop with the compiler warnings
            // Ensure the user properties are not null before accessing them
            if (user.PasswordHash is null || user.UserName is null || user.Email is null) {
                // Handle the case where one or more properties are null
                return Page();
            }

            var passwordHasher = new PasswordHasher<Company>();
            var passwordResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Credential.Password);

            if (passwordResult != PasswordVerificationResult.Success) {
                ModelState.AddModelError(string.Empty, "Wrong Email or Password");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, Credential.Password, isPersistent: true, lockoutOnFailure: false);

            if (!result.Succeeded) {
                ModelState.AddModelError(string.Empty, "Invalid login attempt. "+result.ToString());
                return Page();
            }

            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };

            var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
            var authProperties = new AuthenticationProperties {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.UtcNow
            };

<<<<<<< HEAD:webserver/Pages/Account/Login.cshtml.cs
            await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);
=======
            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
            };
            
            var roles = await _userManager.GetRolesAsync(user);
            foreach(var role in roles){
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
>>>>>>> df4c939 (Pages Authorizations Arranged):webserver/Pages/Accounts/Login.cshtml.cs

            string MyConnection= "server=localhost;port=3306;database=Guacamole;user=root;password=xpvista7810";
            string query = "UPDATE BZEAccounts SET LastLogin = @LastLogin WHERE Id = @Id";

            using (MySqlConnection connection = new MySqlConnection(MyConnection)) {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@LastLogin", DateTime.UtcNow);
                command.Parameters.AddWithValue("@Id", user.Id);

                connection.Open();
                command.ExecuteNonQuery();
            }

            // Redirect to the desired page after successful login
            return RedirectToPage("/Privacy");
        }
    }

    public class Credential {

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

    }
}