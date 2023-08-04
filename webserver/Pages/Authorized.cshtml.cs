using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using webserver.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace webserver.Pages
{

    [Authorize(Policy = Common.BZELevelPolicy)]
    public class Authorized : PageModel
    {
        //private readonly ILogger<Authorized> _logger;

        public Authorized(/*ILogger<Authorized> logger*/)
        {
            //_logger = logger;
        }

        public IActionResult OnGet()
        {
            Console.WriteLine("debug is on the console");
            Debug.WriteLine("debug is on de table");

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Accounts/Login");
            }

            return Page();
        }
    }
}