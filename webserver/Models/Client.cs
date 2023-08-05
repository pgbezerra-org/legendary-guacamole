using Microsoft.AspNetCore.Identity;

namespace webserver.Models;

public class Client : IdentityUser {

    // Additional properties specific to Client
    public string Occupation { get; set; } = string.Empty;

}