using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using webserver.Data;

namespace webserver.Pages
{

    [Authorize(Common.BZELevelPolicy)]
    public class Authorized : PageModel
    {
        private readonly ILogger<Authorized> _logger;

        public Authorized(ILogger<Authorized> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}