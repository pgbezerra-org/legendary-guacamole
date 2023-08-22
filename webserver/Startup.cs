using webserver.Data;
using webserver.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace webserver {

    public class Startup {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {

            services.AddDefaultIdentity<BZEAccount>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<WebserverContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityCore<BZEmployee>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<WebserverContext>();
            services.AddIdentityCore<Company>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<WebserverContext>();
            services.AddIdentityCore<Client>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<WebserverContext>();

            services.AddRazorPages();

            services.AddMvc();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.LoginPath = "/Accounts/Login";
                    options.AccessDeniedPath = "/Accounts/Login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.SlidingExpiration = true;
                    options.Cookie.Name = Common.BZE_Cookie;
                });

            services.AddAuthorization();

            services.AddControllersWithViews();
            
            string connectionString="MyConnection";
            services.AddDbContext<WebserverContext>(options =>
                options.UseMySQL(connectionString));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

            if (!env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // Add this line to enable authentication
            app.UseAuthorization();

            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
