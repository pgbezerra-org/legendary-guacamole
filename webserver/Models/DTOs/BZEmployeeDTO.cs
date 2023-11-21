namespace webserver.Models.DTOs;
/// <summary>
/// BZEmployee DTO, with the main IdentityUser fields
/// </summary>
public class BZEmployeeDTO {

    /// <summary>
    /// Employee's monthly salary
    /// </summary>
    public float Salary;

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
    /// BZEmployeeDTO constructor
    /// The ID field is only ever used by the API, and for finding the Employee
    /// It is NOT used in POST operations. 
    /// The requester shall have a valid Id when making GET, PATCH and DELETE operations. Otherwise will result in BadRequest
    /// </summary>
    public BZEmployeeDTO(string id, string username, string email, string phonenumber, float salary){

        Id = id;
        UserName = username;
        Email = email;
        PhoneNumber = phonenumber;
        
        Salary = salary;
    }

    /// <summary>
    /// Explicit Conversor, from BZEmployeeDTO to BZEmployee
    /// It simply maps the DTO values to the BZE
    /// All fields of the BZEmployee that are inherited from the IdentityUser are properly handled by EntityFramework or the API
    /// </summary>
    public static explicit operator BZEmployee(BZEmployeeDTO dto){
        return new BZEmployee {
            Id = dto.Id,
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,

            salary = dto.Salary
        };
    }

    /// <summary>
    /// Explicit Conversor, from BZEmployee to BZEmployeeDTO
    /// It simply maps the values to the DTO, as it is a subset of the BZE
    /// </summary>
    public static explicit operator BZEmployeeDTO(BZEmployee employee){
        return new BZEmployeeDTO(employee.Id, employee.UserName!, employee.Email!, employee.PhoneNumber!, employee.salary);
    }
}