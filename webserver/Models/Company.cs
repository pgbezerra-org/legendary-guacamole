using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace webserver.Models;

public class Company : BZEAccount {

    public Company() {
        RealEstates = new List<RealEstate> { new RealEstate() };
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