using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using webserver.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace webserver.Data
{
    public class webserverContext : IdentityDbContext
    {

        public webserverContext (DbContextOptions<webserverContext> options)
            : base(options)
        {

        }

        public DbSet<Company> Company { get; set; } = default!;

        public DbSet<Client> Clients { get; set; } = default!;

        public DbSet<RealEstate> RealEstates { get; set; } = default!;
    }
}