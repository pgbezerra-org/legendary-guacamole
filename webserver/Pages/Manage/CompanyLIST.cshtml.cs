using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using MySqlConnector;
using webserver.Models;

namespace webserver.Pages.Manage 
{
    [Authorize(Roles=Common.BZE_Role)]
    public class CompanyLIST : PageModel {
        public List<CompanyINFO> listComps=new List<CompanyINFO>();
        
        public CompanyLIST() {
            
        }
        
        public void OnGet() {
            string MyConnection= "server=localhost;port=3306;database=Guacamole;user=root;password=xpvista7810";

            using (MySqlConnection connection=new MySqlConnection(MyConnection)){
                connection.Open();

                string allComps = "SELECT c.*, u.UserName FROM Company c JOIN AspNetUsers u ON c.Id = u.Id";
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

public class CompanyINFO{
    public string Id { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

}