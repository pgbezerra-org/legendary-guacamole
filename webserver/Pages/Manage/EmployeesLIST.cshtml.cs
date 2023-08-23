using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using MySqlConnector;
using webserver.Models;

namespace webserver.Pages.Manage 
{
    [Authorize(Roles=Common.BZE_Role)]
    public class EmployeesLIST : PageModel {
        public List<EmployeeINFO> listEmpls=new List<EmployeeINFO>();

        public int rowCount, index, size;
        
        public EmployeesLIST() {
            
        }
        
        public void OnGet(int pageIndex =1, int pageSize=5, string orderby="UserName") {
            string MyConnection= "server=localhost;port=3306;database=Guacamole;user=root;password=xpvista7810";

            using (MySqlConnection connection = new MySqlConnection(MyConnection)) {
                connection.Open();
                string countQuery = "SELECT COUNT(*) FROM (SELECT b.*, u.UserName FROM BZEmployees b JOIN AspNetUsers u ON b.Id = u.Id) AS subquery";
                using (MySqlCommand command = new MySqlCommand(countQuery, connection)) {
                    rowCount = Convert.ToInt32(command.ExecuteScalar());
                    index=pageIndex;
                    size=pageSize;
                }
            }

            using (MySqlConnection connection=new MySqlConnection(MyConnection)){
                connection.Open();
                int offset=(pageIndex-1)*pageSize;
                string allComps = $"SELECT b.*, u.UserName FROM BZEmployees b JOIN AspNetUsers u ON b.Id = u.Id ORDER BY {orderby} LIMIT {pageSize} OFFSET {offset}"; //limit search at a later date...
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