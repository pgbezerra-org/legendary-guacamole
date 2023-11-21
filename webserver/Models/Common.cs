namespace webserver.Models;

/// <summary>
/// Guacamole's variables used all across the project
/// </summary>
public static class Common {

    /// <summary>
    /// BZE Cookie name
    /// </summary>
    public const string BZE_Cookie = "BZE_Auth";
    /// <summary>
    /// Company Cookie name
    /// </summary>
    public const string Company_Cookie = "Company_Auth";
    /// <summary>
    /// Client Cookie name
    /// </summary>
    public const string Client_Cookie = "Client_Auth";
    
    /// <summary>
    /// BZE Role name
    /// Keep in mind that for now all BZEmployees can CRUD any and every User or Real Estate
    /// </summary>
    public const string BZE_Role = "BZE_Role";
    /// <summary>
    /// Company Role name
    /// </summary>
    public const string Company_Role = "Company_Role";
    /// <summary>
    /// Client Role name
    /// As by design, Clients can Read, and only read, Companies and Real Estates
    /// </summary>
    public const string Client_Role = "Client_Role";

    /// <summary>
    /// Definitions of Real Estate Properties
    /// </summary>
    public enum HouseType {
        /// <summary>
        /// Independent Real Estate
        /// </summary>
        house = 1,
        /// <summary>
        /// House within a condominium
        /// </summary>
        condominium,
        /// <summary>
        /// Apartment
        /// </summary>
        apartment
    }
}    