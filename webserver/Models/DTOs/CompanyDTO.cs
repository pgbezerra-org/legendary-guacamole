namespace webserver.Models.DTOs;

public class CompanyDTO {

    //Company Fields
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    //IdentityUser Fields
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public CompanyDTO(string id, string username, string email, string phonenumber, string country, string state, string city){

        Id = id;
        UserName = username;
        Email = email;
        PhoneNumber = phonenumber;

        Country = country;
        State = state;
        City = city;
    }

    public static explicit operator Company(CompanyDTO DTO){
        return new Company {
            Id = DTO.Id,
            UserName = DTO.UserName,
            Email = DTO.Email,
            PhoneNumber = DTO.PhoneNumber,

            Country = DTO.Country,
            State = DTO.State,
            City = DTO.City
        };
    }

    public static explicit operator CompanyDTO(Company c){
        return new CompanyDTO(c.Id, c.UserName!, c.Email!, c.PhoneNumber!, c.Country, c.State, c.City);
    }
}