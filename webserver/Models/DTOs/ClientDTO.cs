namespace webserver.Models.DTOs;
public class ClientDTO {

    //Insert Client specific fields
    public string Occupation { get; set; } = string.Empty;

    //Insert IdentityUser fields
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public ClientDTO(string occupation, string id, string username, string email, string phonenumber){
        Occupation = occupation;

        Id = id;
        UserName = username;
        Email = email;
        PhoneNumber = phonenumber;
    }

    public static explicit operator Client(ClientDTO dto){
        return new Client {
            Occupation = dto.Occupation,

            Id = dto.Id,
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };
    }

    public static explicit operator ClientDTO(Client client){
        return new ClientDTO (
            client.Occupation, client.Id, client.UserName!, client.Email!, client.PhoneNumber!
        );
    }    
}