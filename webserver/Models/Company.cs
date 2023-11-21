using System.ComponentModel.DataAnnotations;

namespace webserver.Models;
/// <summary>
/// Company class
/// Such users can only be registered by BZEmployees
/// Companies can CRUD their own Real Estates, as well as register new Clients
/// </summary>
public class Company : BZEAccount {

    /// <summary>
    /// Class constructor
    /// These are applied when inserting to the database
    /// We create a new List for navigation, but clear it to avoid having an empty Real Estate
    /// </summary>
    public Company(){
        RealEstates = new List<RealEstate>();
        RealEstates.Clear();
    }

    /// <summary>
    /// Company constructor with parameters for creating specific companies
    /// </summary>
    public Company(string id, string username, string email, string phonenumber, string country, string state, string city){

        RealEstates = new List<RealEstate>();

        Id = id; UserName = username; Email = email; PhoneNumber = phonenumber;

        Country = country; State = state; City = city;
    }

    /// <summary>
    /// The Country the company is based at
    /// Those are not yet enumerated. We trust the employee to register it properly
    /// </summary>
    [StringLength(30, MinimumLength = 3)]
    [Required]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// The State or Province the company is based at
    /// Some countries do not have States. In such case, this string will be empty
    /// 
    /// Those are not yet enumerated. We trust the employee to register it properly
    /// </summary>
    [StringLength(25, MinimumLength = 3)]
    [Required]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// The City the Company is based at
    /// Real Estates without a city in it's address are assumed to be in the Company's City
    /// 
    /// Those are not yet enumerated. We trust the employee to register it properly
    /// </summary>
    [StringLength(25, MinimumLength = 3)]
    [Required]
    public string City { get; set; } = string.Empty;

    // Navigation property for the related real estates
    public ICollection<RealEstate> RealEstates { get; set; }
}