using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using webserver.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace webserver.Data
{
    public class webserverContext : IdentityDbContext {

        public webserverContext (DbContextOptions<webserverContext> options)
            : base(options)
        {

        }

        public DbSet<webserver.Models.Company> Company { get; set; } = default!;
    }
}