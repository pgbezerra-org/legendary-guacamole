using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace webserver.Models
{
    public class BZEAccount : IdentityUser
    {
        public DateTime LastLogin { get; set; }
    }
}