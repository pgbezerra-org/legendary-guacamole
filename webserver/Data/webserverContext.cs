using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using webserver.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace webserver.Data
{
    public class webserverContext : IdentityDbContext<Company> {

        public webserverContext (DbContextOptions<webserverContext> options)
            : base(options)
        {

        }

        public DbSet<BZEmployee> BZEmployees { get; set; } = default!;

        public DbSet<webserver.Models.Company> Company { get; set; } = default!;
    }
}