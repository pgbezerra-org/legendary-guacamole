namespace webserver.Models.DTOs;
public class BZEmployeeDTO {

    public float Salary;

    //IdentityUser Fields
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// User's real name
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public BZEmployeeDTO(string id, string username, string fullname, string email, string phonenumber, float salary){

        Id = id;
        UserName = username;
        FullName = fullname;
        Email = email;
        PhoneNumber = phonenumber;
        
        Salary = salary;
    }

    public static explicit operator BZEmployee(BZEmployeeDTO dto){
        return new BZEmployee {
            Id = dto.Id,
            UserName = dto.UserName,
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,

            salary = dto.Salary
        };
    }

    public static explicit operator BZEmployeeDTO(BZEmployee employee){
        return new BZEmployeeDTO(employee.Id, employee.UserName!, employee.FullName!, employee.Email!, employee.PhoneNumber!, employee.salary);
    }
}