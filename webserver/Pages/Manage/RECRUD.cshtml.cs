using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using webserver.Data;

namespace webserver.Pages.Manage
{
    [Authorize]
    
    public class RECRUD : PageModel
    {
        public List<RealEstateINFO> listREINFOs=new List<RealEstateINFO>();
        
        public RECRUD() {
            
        }

        public void OnGet()
        {
            string MyConnection= "server=localhost;port=3306;database=Guacamole;user=root;password=xpvista7810";

            using (MySqlConnection connection=new MySqlConnection(MyConnection)){
                connection.Open();
                string allREs = "SELECT RealEstates.*, AspNetUsers.* FROM RealEstates INNER JOIN AspNetUsers ON RealEstates.CompanyId = AspNetUsers.Id"; //limit search at a later date...
                using (MySqlCommand command = new MySqlCommand(allREs, connection)){
                    using (MySqlDataReader reader=command.ExecuteReader()){
                        while(reader.Read()){
                            RealEstateINFO REINFO = new RealEstateINFO();
                            REINFO.Id=reader.GetInt32(0);
                            REINFO.Name=reader.GetString(1);
                            REINFO.Address=reader.GetString(2);
                            REINFO.Price=(decimal)reader.GetFloat(3);
                            REINFO.CompanyId=reader.GetString(4);
                            REINFO.CompanyName = reader.GetString(reader.GetOrdinal("UserName"));
                            //Console.WriteLine(reader.GetString());
                            listREINFOs.Add(REINFO);
                        }
                    }
                }
            }
        }

        
    }
}

public class RealEstateINFO{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }

    // Foreign key to associate the RealEstate with a Company
    public string CompanyId { get; set; }=string.Empty;
    public string CompanyName { get; set; }=string.Empty;
}