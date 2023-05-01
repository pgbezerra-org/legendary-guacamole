using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Threading;

using Azure.Identity;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;

using Microsoft.Build.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RazorPagesMovie.Pages.Account {

    public class LoginModel : PageModel {

        [BindProperty]
        public Credential Credential { get; set; } = new Credential();

        public void OnGet() {
            
        }

        public async Task<IActionResult> OnPostAsync() {

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