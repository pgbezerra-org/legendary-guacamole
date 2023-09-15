using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace webserver.Pages.Manage
{
    [Authorize]
    public class Overview : PageModel
    {
        private readonly ILogger<Overview> _logger;

        public Overview(ILogger<Overview> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}