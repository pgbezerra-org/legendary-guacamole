using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using webserver.Models;
using MySqlConnector;

namespace webserver.Pages.Manage
{
    public class RECRUD : PageModel
    {
        public List<RealEstateINFO> listREINFOs=new List<RealEstateINFO>();
        private readonly ILogger<RECRUD> _logger;        

        public RECRUD(ILogger<RECRUD> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            string MyConnection= "server=localhost;port=3306;database=Guacamole;user=root;password=xpvista7810";

            using (MySqlConnection connection=new MySqlConnection(MyConnection)){
                connection.Open();
                string allREs="SELECT * FROM RealEstates"; //limit search at a later date...
                using (MySqlCommand command = new MySqlCommand(allREs, connection)){
                    using (MySqlDataReader reader=command.ExecuteReader()){
                        while(reader.Read()){
                            RealEstateINFO REINFO = new RealEstateINFO();
                            REINFO.Id=reader.GetInt32(0);
                            REINFO.Name=reader.GetString(1);
                            REINFO.Address=reader.GetString(2);
                            REINFO.Price=(decimal)reader.GetFloat(3);
                            REINFO.CompanyId=reader.GetString(4);
    
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