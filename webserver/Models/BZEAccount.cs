using Microsoft.AspNetCore.Identity;

namespace webserver.Models;

/// <summary>
/// This class is inherited by the User's in Guacamole
/// This simplifies implementing Guacamole Users. We can also trust them to always have the given fields
/// </summary>
public class BZEAccount : IdentityUser {

    /// <summary>
    /// User's Last Login time
    /// Assigned to the Registration moment and updated once the user logs in, if ever
    /// </summary>
    public DateTime LastLogin { get; set; }
    
    /// <summary>
    /// BZEAccount constructor
    /// Keep in mind the Last Login is assigned at this moment,
    /// and thus it's effectively the User's "Last Login" until he logs in for the first time
    /// </summary>
    public BZEAccount(){
        LastLogin = DateTime.UtcNow;
    }
}