using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;

namespace RazorPagesMovie.Pages.Account {

    public class LoginModel : PageModel {

        [BindProperty]
        public Credential Credential { get; set; } = new Credential();

        public void OnGet() {
            
        }

        public async Task<IActionResult> OnPost() {

            if (!ModelState.IsValid) {
                return Page();
            }

            if(Credential.UserName== "admin" && Credential.Password == "password") {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, "admin"), new Claim(ClaimTypes.Email, "admin@mywebsite.com") };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal claimsPrincipal=new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                return RedirectToPage("/Index");
            }

            return Page();
        }
    }

    public class Credential {

        [System.ComponentModel.DataAnnotations.Required]
        public string UserName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

    }
}