using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using MySqlConnector;

namespace webserver.Pages.Manage 
{
    [Authorize]    
    public class ClientCRUD : PageModel {
        public List<ClientINFO> listClients=new List<ClientINFO>();
        
        public ClientCRUD() {
            
        }
        
        public void OnGet() {
            string MyConnection= "server=localhost;port=3306;database=Guacamole;user=root;password=xpvista7810";

            using (MySqlConnection connection=new MySqlConnection(MyConnection)){
                connection.Open();

                string allComps = "SELECT b.*, u.UserName FROM Clients b JOIN AspNetUsers u ON b.Id = u.Id";
                using (MySqlCommand command = new MySqlCommand(allComps, connection)){
                    using (MySqlDataReader reader=command.ExecuteReader()){
                        while(reader.Read()){
                            ClientINFO clientINFO = new ClientINFO();
                            clientINFO.Id=reader.GetString(0);
                            clientINFO.UserName=reader.GetString(1);
                            listClients.Add(clientINFO);
                        }
                    }
                }
            }
        }
    }
}

public class ClientINFO{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}