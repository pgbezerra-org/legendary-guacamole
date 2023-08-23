using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace webserver.Pages
{
    public class About : PageModel
    {
        private readonly ILogger<About> _logger;

        public About(ILogger<About> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}