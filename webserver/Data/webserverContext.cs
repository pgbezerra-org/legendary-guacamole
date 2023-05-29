using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webserver.Models;

namespace webserver.Data
{
    public class webserverContext : IdentityDbContext<Company, IdentityRole, string> {

        public webserverContext (DbContextOptions<webserverContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Company { get; set; } = default!;
    }
}
