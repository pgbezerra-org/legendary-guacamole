using System.ComponentModel.DataAnnotations;
using webserver.Models;
using webserver.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace webserver.Pages.Manage;

[Authorize(Roles=Common.BZE_Role+","+Common.Company_Role)]
public class RealEstateUpdate : PageModel {

    public InputModel Input { get; set; } = new InputModel();
    private readonly WebserverContext _context;
    
    public RealEstateUpdate(WebserverContext context) {
        _context=context;
    }

    public void OnGet(int Id) {

        if(Id==0){
            Redirect();
            return;
        }

        RealEstate? realstate=_context.RealEstates.Find(Id);
        if(realstate!=null){
            Input.Name=realstate.Name;
            Input.Address=realstate.Address;
            Input.Price=realstate.Price;
            Input.Id=Id;

            Input.Company = realstate.OwnerCompany?.UserName ?? "default value";
        }
    }

    void Redirect() {
        RedirectToPage("/Manage/RealEstateCreation");
    }

    public async Task<IActionResult> OnPostAsync() {

        var realEstate=_context.RealEstates.Find(Input.Id);
        if(realEstate!=null){
            realEstate.Address=Input.Address;
            realEstate.Price=Input.Price;
            realEstate.Name=Input.Name;
            //The company it belongs to is not meant to be changeable
            await _context.SaveChangesAsync();
        }else{
            return RedirectToPage("/About");
        }
        
        return RedirectToPage("/Manage/RECRUD");
    }

    public class InputModel {

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }=string.Empty;

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }=string.Empty;

        [Required]
        [Display(Name = "Company")]
        public string Company { get; set; }=string.Empty;

        [Required]
        [Display(Name = "Price")]
        public decimal Price { get; set; } = 1;

        public int Id;
        
    }   
}