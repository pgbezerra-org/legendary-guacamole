using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using webserver.Data;
using webserver.Models;
using webserver.Models.DTOs;

namespace webserver.Pages.Views;
[Authorize]
public class RealEstateProfile : PageModel {

    public WebserverContext context;
    public RealEstate? realEstate;
    public Company? OwnerCompany;

    public RealEstateProfile(WebserverContext _context) {
        context = _context;
    }

    public void OnGet(int id) {

        var client = new HttpClient();
        var endpoint = "http://localhost:5097/api/v1/realestates/unique/"+id;
        var result = client.GetAsync(endpoint).Result;
        var json = result.Content.ReadAsStringAsync().Result;
        
        realEstate = (RealEstate) JsonConvert.DeserializeObject<RealEstate>(json)!;

        if(realEstate==null){
            Console.WriteLine("ERROR 404");
            return;
        }

        OwnerCompany = context.Company.Find(realEstate.CompanyId);
    }
}