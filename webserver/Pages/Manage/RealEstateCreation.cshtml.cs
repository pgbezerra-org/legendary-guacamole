using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using webserver.Models;
using webserver.Data;

namespace webserver.Pages.Manage {

    public class RealEstateCreation : PageModel {

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        
        private readonly WebserverContext context;

        public RealEstateCreation(WebserverContext _context)
        {
            context=_context;
        }

        public class InputModel
        {
            [Required]
            [Display(Name = "Name")]
            public string Name { get; set; }=string.Empty;

            [Required]
            [Display(Name = "Address")]
            public string Address { get; set; }=string.Empty;

            [Required]
            [Display(Name = "Price")]
            public decimal Price { get; set; } = 1;
            
        }

        public async Task<IActionResult> OnPostAsync()
        {

            var realEstate = new RealEstate
            {
                Name = Input.Name,
                Address = Input.Address,
                Price = Input.Price
            };
            
            await context.RealEstate.AddAsync(realEstate);
            await context.SaveChangesAsync();

            return RedirectToPage("/Success");
        }
    }
}