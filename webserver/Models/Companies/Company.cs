using System.ComponentModel.DataAnnotations;

namespace webserver.Models;

public class Company {

    public Company() {

        RealEstates = new List<RealEstate> { new RealEstate() };

    }

    [StringLength(20, MinimumLength = 3)]
    [Required]
    public string Country { get; set; } = string.Empty;

    [StringLength(20, MinimumLength = 3)]
    [Required]
    public string State { get; set; } = string.Empty;

    [StringLength(20, MinimumLength = 3)]
    [Required]
    public string City { get; set; } = string.Empty;

    // Navigation property for the related real estates
    public List<RealEstate> RealEstates { get; set; }
}