using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webserver.Data;
using webserver.Models;

namespace webserver.Pages.Views;
[Authorize]
public class RealEstateProfile : PageModel {

    public WebserverContext _context;
    public RealEstate? realEstate;
    public Company? OwnerCompany;

    public RealEstateProfile(WebserverContext context) {
        _context = context;
    }

    public void OnGet(int id) {

        

        realEstate = _context.RealEstates.Find(id);

        if(realEstate==null){
            Console.WriteLine("ERROR 404");
            return;
        }

        OwnerCompany = _context.Company.Find(realEstate.CompanyId);
    }
}