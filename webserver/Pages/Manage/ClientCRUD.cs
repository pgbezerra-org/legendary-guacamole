using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using MySqlConnector;
using webserver.Utilities;

namespace webserver.Pages.Manage 
{
    [Authorize(Roles=Common.BZE_Role+","+Common.Company_Role)]
    public class ClientCRUD : PageModel {
        
        public List<ClientINFO> listClients = new List<ClientINFO>();

        public int rowCount, index, size;
        
        public ClientCRUD() {
            
        }
        
        public void OnGet(int pageIndex = 1, int pageSize = 5, string orderby = "UserName") {
            string MyConnection = "server=localhost;port=3306;database=Guacamole;user=root;password=xpvista7810";

            using (MySqlConnection connection=new MySqlConnection(MyConnection)){
                connection.Open();

                string countQuery = "SELECT COUNT(*) FROM (SELECT b.*, u.UserName FROM Clients b JOIN AspNetUsers u ON b.Id = u.Id) AS subquery";
                using (MySqlCommand command = new MySqlCommand(countQuery, connection)){
                    rowCount = Convert.ToInt32(command.ExecuteScalar());
                    index = pageIndex;
                    size = pageSize;
                }
            }

            using (MySqlConnection connection = new MySqlConnection(MyConnection)){
                connection.Open();
                int offset=(pageIndex-1)*pageSize;
                string allComps = $"SELECT b.*, u.UserName FROM Clients b JOIN AspNetUsers u ON b.Id = u.Id ORDER BY {orderby} LIMIT {pageSize} OFFSET {offset}";
                using (MySqlCommand command = new MySqlCommand(allComps, connection)){
                    using (MySqlDataReader reader=command.ExecuteReader()){
                        while(reader.Read()){
                            ClientINFO clientINFO = new ClientINFO();
                            clientINFO.Id = reader.GetString(0);
                            clientINFO.Occupation = reader.GetString(1);
                            clientINFO.UserName = reader.GetString(2);
                            listClients.Add(clientINFO);
                        }
                    }
                }
            }
        }
    }

    public class ClientINFO{
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Occupation{get; set; } = string.Empty;
    }
}