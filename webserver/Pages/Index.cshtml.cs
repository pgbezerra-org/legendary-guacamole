using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webserver.Data;
using webserver.Models;
using webserver.Models.DTOs;

namespace webserver.Pages {
    public class IndexModel : PageModel {

        private readonly WebserverContext context;

        [BindProperty]
        public RealEstate[] states {get; set;} = new RealEstate[3];

        public IndexModel(WebserverContext _context) {
            context = _context;
        }

        public void OnGet() {
            
            var query = context.RealEstates
                .OrderByDescending(r => r.Id)
                .Take(3);

            states = query.ToArray();

            //New companies will have an empty Real Estate!

        }
    }
}