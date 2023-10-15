using System.ComponentModel.DataAnnotations;

namespace webserver.Models;
public class Company : BZEAccount {

    public Company(){
        RealEstates = new List<RealEstate>();
    }

    public Company(string id, string username, string email, string phonenumber){
        RealEstates = new List<RealEstate>();
        Id = id; UserName = username; Email = email; PhoneNumber = phonenumber;
    }

    public Company(string id, string username, string email, string phonenumber, string country, string state, string city){

        RealEstates = new List<RealEstate>();

        Id = id; UserName = username; Email = email; PhoneNumber = phonenumber;

        Country = country; State = state; City = city;
    }

    [StringLength(30, MinimumLength = 3)]
    [Required]
    public string Country { get; set; } = string.Empty;

    [StringLength(25, MinimumLength = 3)]
    [Required]
    public string State { get; set; } = string.Empty;

    [StringLength(25, MinimumLength = 3)]
    [Required]
    public string City { get; set; } = string.Empty;

    // Navigation property for the related real estates
    public ICollection<RealEstate> RealEstates { get; set; }
}