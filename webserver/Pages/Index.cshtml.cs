using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webserver.Data;
using webserver.Models;

namespace webserver.Pages
{
    public class IndexModel : PageModel
    {
        private readonly webserver.Data.webserverContext _context;

        public IndexModel(webserver.Data.webserverContext context)
        {
            _context = context;
        }

        public IList<Company> Company { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Company != null)
            {
                Company = await _context.Company.ToListAsync();
            }
        }
    }
}
