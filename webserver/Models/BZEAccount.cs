using Microsoft.AspNetCore.Identity;
using NuGet.Common;

namespace webserver.Models;

public class BZEAccount : IdentityUser {
    public DateTime LastLogin { get; set; }

    public BZEAccount(){
        LastLogin = DateTime.UtcNow;
    }
}