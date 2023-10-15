namespace webserver.Models;

public static class Common {

    public const string BZE_Cookie = "BZE_Auth";
    public const string Company_Cookie = "Company_Auth";
    public const string Client_Cookie = "Client_Auth";
    
    public const string BZE_Role = "BZE_Role";
    public const string Company_Role = "Company_Role";
    public const string Client_Role = "Client_Role";

    public enum HouseType {
        house = 1, condominium, apartment
    }
}    