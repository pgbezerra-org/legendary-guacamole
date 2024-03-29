using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using webserver.Data;
using webserver.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using webserver.Models.DTOs;

namespace webserver.Pages.Views;
[Authorize(Roles =Common.BZE_Role+","+Common.Company_Role)]
public class CompanyProfile : PageModel {

    public WebserverContext context;
    public Company? company;
    public RealEstate[]? states;

    public CompanyProfile(WebserverContext _context) {
        context = _context;
    }

    public void OnGet(string id) {

        var client = new HttpClient();
        var endpoint = "http://localhost:5097/api/v1/company/unique/"+id;
        var result = client.GetAsync(endpoint).Result;
        var json = result.Content.ReadAsStringAsync().Result;
        
        company = (Company) JsonConvert.DeserializeObject<CompanyDTO>(json)!;

        if(company==null){
            Console.WriteLine("ERROR 404");
            return;
        }

        if(company.PhoneNumber==null || company.PhoneNumber==""){
            company.PhoneNumber="Unavailable";
        }

        states = context.RealEstates.Where( r => r.CompanyId==company.Id).ToArray();
    }
}