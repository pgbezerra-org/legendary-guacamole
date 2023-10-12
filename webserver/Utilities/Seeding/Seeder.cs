using Newtonsoft.Json;
using NuGet.Protocol;
using webserver.Models;

namespace webserver.Utilities.Seeding;

//I aimed for a balance between data-realism and code-readability
public class Seeder {
    
    public GeneralSeed generalSeeds;
    public CountrySeed[] countrySeeds;
    
    public Seeder(string generalSeed, string countrySeed){
        generalSeeds = JsonConvert.DeserializeObject<GeneralSeed>(generalSeed.ToJson())!;
        countrySeeds = JsonConvert.DeserializeObject<CountrySeed[]>(countrySeed.ToJson())!;
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
}