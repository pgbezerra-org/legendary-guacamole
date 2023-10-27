using Microsoft.AspNetCore.Identity;
using NuGet.Common;

namespace webserver.Models;

public class BZEAccount : IdentityUser {
    
    public DateTime LastLogin { get; set; }
    /// <summary>
    /// The date at which the User was registered successfully
    /// </summary> <summary>
    public DateTime RegisterDate { get; set; }
    /// <summary>
    /// FullName: the User's actual full name
    /// 
    /// Not to be confused with the UserName
    /// UserName is effectivelly a nickname, and an IdentityUser field
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    public BZEAccount(){
        LastLogin = DateTime.UtcNow;
    }
}