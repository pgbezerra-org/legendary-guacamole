using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NuGet.Protocol;
using webserver.Data;
using webserver.Models;
using webserver.Utilities.Seeding;

namespace webserver.Pages.Seeds;
public class RealEstateSeed : PageModel {

    public string generalseed, countryseed;

    public RealEstate rsrss;
    WebserverContext _context;
    public Company comp;

    public RealEstateSeed(WebserverContext webcon) {

        _context = webcon;

        GeneralSeed gs = new GeneralSeed();
        CountrySeed cs = new CountrySeed();

        gs.Occupation = new string[] { "footballer", "singer"};
        gs.adjective = new string[] { "gay", "womanizer"};
        gs.noun = new string[] { "roger", "dat"};

        //

        cs.countryName = "Brazil";
        cs.peopleName = new string[] { "matheus", "peter"};
        cs.peopleSurname = new string[] { "lacerda", "parker"};

        cs.citySeeds = new CitySeed[2];
        cs.citySeeds[0] = new CitySeed();
        cs.citySeeds[1] = new CitySeed();
        
        cs.citySeeds[0].CityName = "SP";
        cs.citySeeds[0].State = "vulcano";
        cs.citySeeds[0].addresses = new CitySeed.Address[2];
        cs.citySeeds[0].addresses![0] = new CitySeed.Address();
        cs.citySeeds[0].addresses![0].street = new string[2] { "coastline", "backside"};
        cs.citySeeds[0].addresses![0].neighborhood = "bordel";
        cs.citySeeds[0].addresses![0].costMultiplyer = 3m;

        cs.citySeeds[0].addresses![1] = new CitySeed.Address();
        cs.citySeeds[0].addresses![1].street = new string[2] { "something", "else"};
        cs.citySeeds[0].addresses![1].neighborhood = "anything";
        cs.citySeeds[0].addresses![1].costMultiplyer = 3.2m;

        cs.citySeeds[1].CityName = "SP";
        cs.citySeeds[1].State = "maranhao";
        cs.citySeeds[1].addresses = new CitySeed.Address[2];
        cs.citySeeds[1].addresses![0] = new CitySeed.Address();
        cs.citySeeds[1].addresses![0].street = new string[2] { "renascenca", "turu"};
        cs.citySeeds[1].addresses![0].neighborhood = "praia";
        cs.citySeeds[1].addresses![0].costMultiplyer = 1.5m;

        cs.citySeeds[1].addresses![1] = new CitySeed.Address();
        cs.citySeeds[1].addresses![1].street = new string[2] { "bacanga", "vinhais"};
        cs.citySeeds[1].addresses![1].neighborhood = "baixada";
        cs.citySeeds[1].addresses![1].costMultiplyer = 3.6m;
        
        CountrySeed[] csArray = new CountrySeed[2];
        csArray[0] = cs;
        csArray[1] = cs;
        string csArrayS = JsonConvert.SerializeObject(csArray, Formatting.None).ToString();

        generalseed = JsonConvert.SerializeObject(gs,Formatting.None).ToString();
        countryseed = JsonConvert.SerializeObject(cs,Formatting.None).ToString();

        Seeder seeder = new Seeder(generalseed, csArrayS);

        comp = _context.Company.First();

        RealEstate[] states = seeder.GetRealEstates(10, comp);
        
        //rsrss = states[0];
    }

    public void OnGet() {

    }
}