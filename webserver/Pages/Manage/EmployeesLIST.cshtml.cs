using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using MySqlConnector;

namespace webserver.Pages.Manage 
{
    [Authorize(Roles=Common.BZE_Role)]
    public class EmployeesLIST : PageModel {
        public List<EmployeeINFO> listEmpls=new List<EmployeeINFO>();
        
        public EmployeesLIST() {
            
        }
        
        public void OnGet() {
            string MyConnection= "server=localhost;port=3306;database=Guacamole;user=root;password=xpvista7810";

            using (MySqlConnection connection=new MySqlConnection(MyConnection)){
                connection.Open();

                string allComps = "SELECT b.*, u.UserName FROM BZEmployees b JOIN AspNetUsers u ON b.Id = u.Id";
                using (MySqlCommand command = new MySqlCommand(allComps, connection)){
                    using (MySqlDataReader reader=command.ExecuteReader()){
                        while(reader.Read()){
                            EmployeeINFO bzeINFO = new EmployeeINFO();
                            bzeINFO.Id=reader.GetString(0);
                            bzeINFO.UserName=reader.GetString(1);
                            listEmpls.Add(bzeINFO);
                        }
                    }
                }
            }
        }
    }
}

public class EmployeeINFO{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}