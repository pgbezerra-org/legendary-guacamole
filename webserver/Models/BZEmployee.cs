using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace webserver.Models;

public class BZEmployee : IdentityUser {

    public DateTime LastLogin { get; set; } //Just to give it some unique field

}