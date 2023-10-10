namespace webserver.Models;
public class RealEstate {

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public bool rentable { get; set; }
    public int area { get; set; }
    public int percentage { get; set; }
    public int numBedrooms { get; set; }
    public string houseType = string.Empty;

    // Foreign key to associate the RealEstate with a Company
    public string CompanyId { get; set; } = string.Empty;

    // Navigation property
    [System.Text.Json.Serialization.JsonIgnore]
    public Company? OwnerCompany { get; set; }
}