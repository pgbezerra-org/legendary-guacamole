namespace webserver.Models;

public class RealEstate {

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }

    // Foreign key to associate the RealEstate with a Company
    public string CompanyId { get; set; } = string.Empty;
    // Navigation property
    public Company? OwnerCompany { get; set; }
}