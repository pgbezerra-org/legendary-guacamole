using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using webserver.Data;

namespace webserver.Pages.Manage {

    [Authorize]    
    public class RECRUD : PageModel {

        public List<RealEstateINFO> listREINFOs=new List<RealEstateINFO>();

        public int rowCount;
        public int index {get;set;}=1;
        public int size {get;set;}=5;

        private readonly WebserverContext context;
        
        public RECRUD(WebserverContext _context) {
            context=_context;
        }

        public void OnGet(int pageIndex =1, int pageSize=5, string orderby="Price") {
            
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

                string allREs = $"SELECT RealEstates.*, AspNetUsers.* FROM RealEstates INNER JOIN AspNetUsers ON RealEstates.CompanyId = AspNetUsers.Id ORDER BY {orderby} LIMIT {pageSize} OFFSET {offset}";
                //Console.WriteLine(allREs+"   AQUIII MERMAAAAAO ----------- --------\n\n");
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

        public async Task<IActionResult> OnPostDeleteItem(int id) {
            
            var realEstateToRemove = await context.RealEstates.FindAsync(id);
            if (realEstateToRemove != null) {
                context.RealEstates.Remove(realEstateToRemove);
                await context.SaveChangesAsync();
            }
            
            OnGet(index, size, "Price");
            return Page();
        }
    }

    public class RealEstateINFO{
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Price { get; set; } = 1;

        // Foreign key to associate the RealEstate with a Company
        public string CompanyId { get; set; }=string.Empty;
        public string CompanyName { get; set; }=string.Empty;
    }
}