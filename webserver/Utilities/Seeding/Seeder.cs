using Newtonsoft.Json;
using webserver.Models;

namespace webserver.Utilities.Seeding;

//I aimed for a balance between data-realism and code-readability
public class Seeder {
    
    public GeneralSeed generalSeeds;
    public CountrySeed[] countrySeeds;
    
    public Seeder(string generalSeed, string countrySeed){
        generalSeeds = JsonConvert.DeserializeObject<GeneralSeed>(generalSeed.ToString())!;
        countrySeeds = JsonConvert.DeserializeObject<CountrySeed[]>(countrySeed.ToString())!;
    }

    public Company[] GetCompanies(int amount, string[] countries){

        Company[] companies = new Company[amount];

        foreach(Company comp in companies){
            
            //ordena o countrySeed pelo nome dos countries

            //pega um nome dentro desses

            //dá pra company uma cidade desse país

            //dá um username com um adjetivo (ou não), e um nome e substantivo

            //dá um email baseado nisso e com um minimo de randomizacao

            //dá uma senha baseada no username, com no maximo 3 variacoes de senha

        }

        return companies;
    }

    public RealEstate[] GetRealEstates(int amount, Company company) {

        RealEstate[] estates = new RealEstate[amount];
        for(int i=0;i<estates.Length;i++){
            estates[i] = new RealEstate("name", "addr", 100, "comp");
        }

        CountrySeed country;
        CitySeed citySeed;

        Random randomizer = new Random();
        int intSeed = randomizer.Next();

        foreach(RealEstate re in estates){

            re.CompanyId = company.Id;

            country = countrySeeds.Where(c=>c.countryName == company.Country).FirstOrDefault()!;
            citySeed = country.citySeeds!.Where(ct=>ct.CityName == company.City).FirstOrDefault()!;

            CitySeed.Address addr = citySeed.addresses![ intSeed % citySeed.addresses.Length - 1 ];

            Common.HouseType houseType = (Common.HouseType)((intSeed % 3)+1);

            if(houseType == Common.HouseType.house){
                re.houseType = Common.HouseType.house.ToString();
                re.Address = addr.street[ intSeed % addr.street.Length - 1 ] + ", N " + ((intSeed % 50)+1) + ", " + addr.neighborhood;
                re.area = randomizer.Next(60, 360);
            }else if(houseType == Common.HouseType.condominium){
                re.houseType = Common.HouseType.condominium.ToString();
                re.Address = addr.street[ intSeed % addr.street.Length - 1 ] + ", N " + ((intSeed % 200)+1) + ", " + addr.neighborhood;
                re.area = randomizer.Next(30, 180);
                re.percentage = 100;
            }else{
                int numApt = (((intSeed % 12)+1)*10) + ((intSeed % 3) + 1) * 10;    //12 andares + o numero do apartamento

                re.houseType = Common.HouseType.apartment.ToString();
                re.Address = addr.street[ intSeed % addr.street.Length - 1 ] + ", N " + numApt + ", " + addr.neighborhood;
                re.area = randomizer.Next(30, 180);
                
                if(intSeed % 10 == 0){
                    re.percentage = randomizer.Next(0, 99);
                }else{
                    re.percentage = 100;
                }
            }

            re.rentable = houseType != 0;
            
            if(re.area <= 60){
                re.numBedrooms = 2;
            }else if(re.area >= 600){
                re.numBedrooms = 4;
            }else{
                re.numBedrooms = randomizer.Next(3,4);  //wtv
            }

            re.Price = re.numBedrooms * 10000 + re.area * 200;

            if(re.percentage <= 82){
                re.Price *= (decimal)0.7;
            }

            re.Price *= addr.costMultiplyer;            

            if(houseType == Common.HouseType.apartment){
                re.Price *= (decimal)1.5;
            }

            re.Name = country.peopleSurname[intSeed%country.peopleSurname.Length - 1] + "'s ";

            if(re.Price > 600000 || addr.costMultiplyer >= 5){
                re.Name += addr.neighborhood;
            }

            re.Name += houseType.ToString();
        }        

        return estates;
    }
}