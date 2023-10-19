namespace webserver.Utilities.Seeding;
public class CitySeed {
    
    public string CityName = string.Empty;
    public string State = string.Empty;
    public Address[]? addresses;

    public class Address{
        public string neighborhood = string.Empty;
        public string[] street = new string[] { string.Empty };
        public decimal costMultiplyer = 3.0m; //The greater it is, the more expensive real-estates tend to be in that area
    }
}