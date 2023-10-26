using Newtonsoft.Json;

namespace webserver.Models;

/// <summary>
/// Real Estate class Model
/// We elected to send the whole class in the API. THere wasn't much need for a DTO
/// </summary>
public class RealEstate {

    /// <summary>
    /// Simple integer Id
    /// Since it's named 'Id', it's automatically assumed as an Id by .NET and handled as such
    /// Incremental primary Key with a maximum value of 2.147.483.647 
    /// </summary>
    [JsonProperty]
    public int Id { get; set; }
    /// <summary>
    /// Arbitrary Name given by the Real Estate's owner
    /// </summary>
    [JsonProperty]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Address of the Real Estate. Owner's discretion is advised
    /// If the city isn't specified, the Owner Company's City is assumed
    /// </summary>
    [JsonProperty]
    public string Address { get; set; } = string.Empty;
    /// <summary>
    /// Price in local currency
    /// </summary>
    [JsonProperty]
    public decimal Price { get; set; }

    /// <summary>
    /// Foreign Key, Id of the Company that owns the Real Estate
    /// </summary>
    [JsonProperty]
    public string CompanyId { get; set; } = string.Empty;
    /// <summary>
    /// Navigation property. API users should DISREGARD it
    /// </summary>
    [JsonIgnore]
    public Company? OwnerCompany { get; set; }

    /// <summary>
    /// Whether or not the place is up for renting. Will be changed to 'rent price' later
    /// </summary>
    [JsonProperty]
    public bool rentable { get; set; }
    /// <summary>
    /// Total property area. Do not confuse with "construction area"
    /// </summary>
    [JsonProperty]
    public int area { get; set; }
    
    /// <summary>
    /// Value from 0 to 100 of the state of the Real Estate construction
    /// Subjective and thus entrusted to the Owner's judgement
    /// Project brands values less than 50 as "in the blueprint"
    /// </summary>
    [JsonProperty]
    public int percentage { get; set; }
    /// <summary>
    /// Number of bedrooms. Does set bedrooms with and without bathrooms apart yet
    /// </summary>
    [JsonProperty]
    public int numBedrooms { get; set; }
    /// <summary>
    /// House, Apartment or Condominium
    /// MySQL accepts only strings, but we do convert from an 'enum Utilities.Common.HouseType'
    /// </summary>
    [JsonProperty]
    public string houseType = string.Empty;

    /// <summary>
    /// RealEstate Constructor
    /// All values are evaluated by the API
    /// </summary>
    public RealEstate(string name, string address, decimal price, string companyId){
        Name = name;
        Address = address;
        Price = price;
        CompanyId = companyId;

        area = 50;
        percentage = 100;
        numBedrooms = 1;
        houseType = Common.HouseType.house.ToString();
    }
}