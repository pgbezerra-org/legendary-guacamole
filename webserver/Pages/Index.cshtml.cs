using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webserver.Data;

namespace webserver.Pages {
    public class IndexModel : PageModel {

        private readonly WebserverContext context;

        [BindProperty]
        public RealEstateINFO[] states {get; set;}=new RealEstateINFO[3];

        public IndexModel(WebserverContext _context) {
            context = _context;
        }

        public void OnGet() {
            
            var query = context.RealEstates
                .OrderByDescending(r => r.Id)
                .Take(3)
                .Select(r => new RealEstateINFO { Name = r.Name, Address = r.Address, Price = r.Price });

            states = query.ToArray();

        }

        public class RealEstateINFO{
            public string Name { get; set;}=string.Empty;
            public string Address { get; set;}=string.Empty;

            public decimal Price;
        }
    }
}