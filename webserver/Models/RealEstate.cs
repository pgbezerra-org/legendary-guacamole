namespace webserver.Models;

public class RealEstate {

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public bool rentable = false;

    public int area = 16;

    public int percentage = 100;

    public int numBedrooms = 1;

    public HouseType houseType;

    // Foreign key to associate the RealEstate with a Company
    public string CompanyId { get; set; }=string.Empty;
    // Navigation property
    public Company? OwnerCompany { get; set; }
}

public enum HouseType {
    house = 1, condominium, apartment
}