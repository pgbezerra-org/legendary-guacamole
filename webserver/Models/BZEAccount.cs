using Microsoft.AspNetCore.Identity;

namespace webserver.Models
{
    public class BZEAccount : IdentityUser
    {
        public DateTime LastLogin { get; set; }
    }
}