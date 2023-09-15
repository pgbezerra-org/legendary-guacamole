using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webserver.Models;
namespace webserver.Models;

public class Company : IdentityUser {

    [StringLength(20, MinimumLength = 3)]
    [Required]
    public string Country { get; set; } = string.Empty;

    [StringLength(20, MinimumLength = 3)]
    [Required]
    public string State { get; set; } = string.Empty;

    [StringLength(20, MinimumLength = 3)]
    [Required]
    public string City { get; set; } = string.Empty;
}