using Newtonsoft.Json;

namespace webserver.Models;
public class RealEstate {

    [JsonProperty]
    public int Id { get; set; }
    [JsonProperty]
    public string Name { get; set; } = string.Empty;
    [JsonProperty]
    public string Address { get; set; } = string.Empty;
    [JsonProperty]
    public decimal Price { get; set; }

    [JsonProperty]
    public string CompanyId { get; set; } = string.Empty;
    // Navigation property
    [JsonIgnore]
    public Company? OwnerCompany { get; set; }

    [JsonProperty]
    public bool rentable { get; set; }
    [JsonProperty]
    public int area { get; set; }
    [JsonProperty]
    public int percentage { get; set; }
    [JsonProperty]
    public int numBedrooms { get; set; }
    [JsonProperty]
    public string houseType = string.Empty;

    public RealEstate(string name, string address, decimal price, string companyId){
        Name = name;
        Address = address;
        Price = price;
        CompanyId = companyId;

        area = 50;
        percentage = 100;
        numBedrooms = 1;
        houseType = Utilities.Common.HouseType.house.ToString();
    }
}