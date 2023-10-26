namespace webserver.Models.DTOs;
/// <summary>
/// Client DTO, with the main IdentityUser fields
/// </summary>
public class ClientDTO {

    /// <summary>
    /// Client's Occupation
    /// Currently just an excuse for a Client specific field
    /// </summary>
    public string Occupation { get; set; } = string.Empty;

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
    /// ClientDTO constructor
    /// The ID field is only ever used by the API, and for finding the Client
    /// It is NOT used in POST operations. 
    /// The requester shall have a valid Id when making GET, PATCH and DELETE operations. Otherwise will result in BadRequest
    /// </summary>
    public ClientDTO(string id, string username, string email, string phonenumber, string occupation){
        Id = id;
        UserName = username;
        Email = email;
        PhoneNumber = phonenumber;

        Occupation = occupation;
    }

    /// <summary>
    /// Explicit Conversor, from ClientDTO to Client
    /// It simply maps the DTO values to the Client
    /// All fields of the Client fields that are inherited from the IdentityUser are properly handled by EntityFramework or the API
    /// </summary>
    public static explicit operator Client(ClientDTO dto){
        return new Client {
            Id = dto.Id,
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,

            Occupation = dto.Occupation
        };
    }

    /// <summary>
    /// Explicit Conversor, from Client to ClientDTO
    /// It simply maps the values to the DTO, as it is a subset of the Client
    /// </summary>
    public static explicit operator ClientDTO(Client client){
        return new ClientDTO (
            client.Id, client.UserName!, client.Email!, client.PhoneNumber!, client.Occupation
        );
    }    
}