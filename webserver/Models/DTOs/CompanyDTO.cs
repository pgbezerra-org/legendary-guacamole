namespace webserver.Models.DTOs;
/// <summary>
/// Company DTO, with the main IdentityUser fields
/// </summary>
public class CompanyDTO {

    /// <summary>
    /// Country's name on which the Company's originated from
    /// 
    /// Country, State and City are not enumerated for now
    /// We trust the User to type correctly, but those fields can be updated via the API
    /// </summary>
    public string Country { get; set; } = string.Empty;
    /// <summary>
    /// State or Province the Company is from. Currently not Enumerated
    /// Empty if it doesn't apply to the Country. Examples: Spain, Germany, Vatican
    /// </summary>
    public string State { get; set; } = string.Empty;
    /// <summary>
    /// City the Company is from
    /// If the Real Estates the Company owns do not have the City name in it's address, we assume it's the Company's
    /// </summary>
    public string City { get; set; } = string.Empty;

    //IdentityUser Fields

    /// <summary>
    /// GUID Id. Given by EntityFramework at the User's Creation
    /// Let E.F handle it, only write when sending a DTO
    /// </summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// UserName. Can not contain spaces. Effectively a Nickname for the User
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// Registration Email. Can not be changed once the User is registered
    /// </summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// PhoneNumber string
    /// EntityFramework validates it but does it is not authenticated
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// CompanyDTO constructor
    /// The ID field is only ever used by the API, and for finding the Company
    /// It is NOT used in POST operations. 
    /// The requester shall have a valid Id when making GET, PATCH and DELETE operations. Otherwise will result in BadRequest
    /// </summary>
    public CompanyDTO(string id, string username, string email, string phonenumber, string country, string state, string city){

        Id = id;
        UserName = username;
        Email = email;
        PhoneNumber = phonenumber;

        Country = country;
        State = state;
        City = city;
    }

    /// <summary>
    /// Explicit Conversor, from CompanyDTO to Company
    /// It simply maps the DTO values to the Company
    /// All fields of the Company that are inherited from the IdentityUser are properly handled by EntityFramework or the API
    /// </summary>
    public static explicit operator Company(CompanyDTO dto){
        return new Company (
            dto.Id, dto.UserName, dto.Email, dto.PhoneNumber,

            dto.Country, dto.State, dto.City
        );
    }

    /// <summary>
    /// Explicit Conversor, from Client to ClientDTO
    /// It simply maps the values to the DTO, as it is a subset of the Client
    /// </summary>
    public static explicit operator CompanyDTO(Company c){
        return new CompanyDTO(c.Id, c.UserName!, c.Email!, c.PhoneNumber!, c.Country, c.State, c.City);
    }
}