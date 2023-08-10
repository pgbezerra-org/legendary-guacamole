using Microsoft.EntityFrameworkCore;
using webserver.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace webserver.Data
{
    public class WebserverContext : IdentityDbContext {

        public WebserverContext (DbContextOptions<WebserverContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder){
            base.OnModelCreating(builder);
            
            builder.Entity<BZEAccount>(entity => { entity.ToTable("BZEAccounts"); });
            builder.Entity<Company>(entity => { entity.ToTable("Company"); });
            builder.Entity<Client>(entity => { entity.ToTable("Clients"); });
            
        }

        public DbSet<BZEAccount> BZEAccounts {get;set;}=default!;

        public DbSet<BZEmployee> BZEmployees { get; set; } = default!;
        public DbSet<Company> Company { get; set; } = default!;
        public DbSet<Client> Clients { get; set; } = default!;

        public DbSet<RealEstate> RealEstates { get; set; } = default!;
    }
}