using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using MySqlConnector;
using webserver.Models;

namespace webserver.Pages.Manage  {
    [Authorize(Roles=Common.BZE_Role)]
    public class CompanyLIST : PageModel {

        public class CompanyINFO{
        public string Id { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

    }

        public List<CompanyINFO> listComps=new List<CompanyINFO>();

        public int rowCount, index, size;
        
        public CompanyLIST() {
            
        }
        
        public void OnGet(int pageIndex =1, int pageSize=5, string orderby="UserName") {
            string MyConnection= "server=localhost;port=3306;database=Guacamole;user=root;password=xpvista7810";

            using (MySqlConnection connection = new MySqlConnection(MyConnection)) {
                connection.Open();
                string countQuery = "SELECT COUNT(*) FROM (SELECT c.*, u.UserName FROM Company c JOIN AspNetUsers u ON c.Id = u.Id) AS subquery";
                using (MySqlCommand command = new MySqlCommand(countQuery, connection)) {
                    rowCount = Convert.ToInt32(command.ExecuteScalar());
                    index=pageIndex;
                    size=pageSize;
                }
            }

            using (MySqlConnection connection=new MySqlConnection(MyConnection)){
                connection.Open();
                int offset=(pageIndex-1)*pageSize;
                string allComps = $"SELECT c.*, u.UserName FROM Company c JOIN AspNetUsers u ON c.Id = u.Id ORDER BY {orderby} LIMIT {pageSize} OFFSET {offset}";
                using (MySqlCommand command = new MySqlCommand(allComps, connection)){
                    using (MySqlDataReader reader=command.ExecuteReader()){
                        while(reader.Read()){
                            CompanyINFO compInfo = new CompanyINFO();
                            compInfo.Id=reader.GetString(0);
                            compInfo.Country=reader.GetString(1);
                            compInfo.State=reader.GetString(2);
                            compInfo.City=reader.GetString(3);
                            compInfo.UserName=reader.GetString(4);
                            listComps.Add(compInfo);
                        }
                    }
                }
            }
        }
    }
}