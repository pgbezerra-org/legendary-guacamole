namespace webserver.Models.DTOs;
public class ClientDTO {

    //Insert Client specific fields
    public string Occupation { get; set; } = string.Empty;

    //Insert IdentityUser fields
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public ClientDTO(string id, string username, string email, string phonenumber, string occupation){
        Id = id;
        UserName = username;
        Email = email;
        PhoneNumber = phonenumber;

        Occupation = occupation;
    }

    public static explicit operator Client(ClientDTO dto){
        return new Client {
            Id = dto.Id,
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,

            Occupation = dto.Occupation
        };
    }

    public static explicit operator ClientDTO(Client client){
        return new ClientDTO (
            client.Id, client.UserName!, client.Email!, client.PhoneNumber!, client.Occupation
        );
    }    
}