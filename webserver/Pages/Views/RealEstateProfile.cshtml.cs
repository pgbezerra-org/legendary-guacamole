using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webserver.Data;
using webserver.Models;

namespace webserver.Pages.Views {
    public class RealEstateProfile : PageModel {
        public WebserverContext context;

        public RealEstate? realEstate;
        public Company? OwnerCompany;

        public RealEstateProfile(WebserverContext _context) {
            context=_context;
        }

        public void OnGet(int id) {

            realEstate=context.RealEstates.Find(id);

            if(realEstate==null){
                Console.WriteLine("ERROR 404");
                return;
            }

            OwnerCompany=context.Company.Find(realEstate.CompanyId);
        }
    }
}