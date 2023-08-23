using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace webserver.Pages.Manage {
    [Authorize]
    
    public class RECRUD : PageModel {

        public List<RealEstateINFO> listREINFOs=new List<RealEstateINFO>();

        public int rowCount, index, size;
        
        public RECRUD() {
            
        }

        public void OnGet(int pageIndex =1, int pageSize=5) {
            string MyConnection= "server=localhost;port=3306;database=Guacamole;user=root;password=xpvista7810";

            using (MySqlConnection connection=new MySqlConnection(MyConnection)){
                connection.Open();

                string countQuery = "SELECT COUNT(*) FROM RealEstates";
                using (MySqlCommand command = new MySqlCommand(countQuery, connection)){
                    rowCount = Convert.ToInt32(command.ExecuteScalar());
                    index=pageIndex;
                    size=pageSize;
                }
            }
            
            using (MySqlConnection connection=new MySqlConnection(MyConnection)){
                connection.Open();
                int offset=(pageIndex-1)*pageSize;
                string allREs = $"SELECT RealEstates.*, AspNetUsers.* FROM RealEstates INNER JOIN AspNetUsers ON RealEstates.CompanyId = AspNetUsers.Id LIMIT {pageSize} OFFSET {offset}"; //limit search at a later date...
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