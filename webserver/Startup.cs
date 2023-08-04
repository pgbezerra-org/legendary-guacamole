using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webserver.Data;
using webserver.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace webserver {

    public class Startup {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {

            services.AddIdentity<BZEmployee, IdentityRole>()
        .AddEntityFrameworkStores<WebserverContext>()
        .AddDefaultTokenProviders();

            services.AddMvc();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath="/Accounts/Login";
        options.AccessDeniedPath = "/Accounts/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.Name = Common.BZECookie;
    });

            services.AddControllersWithViews();

            services.AddDbContext<WebserverContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("MyConnection")));

            services.AddAuthentication(Common.BZECookie).AddCookie(options=>
            {
                options.Cookie.Name=Common.BZECookie;
                options.LoginPath="/Accounts/Login";
                options.AccessDeniedPath = "/Accounts/Login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            });

            services.AddAuthorization(options=>
            {
                options.AddPolicy(Common.BZELevelPolicy, policy=>{
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(Common.BZECookie);
                });
            });

            services.ConfigureApplicationCookie(options=>{
                options.Cookie.Name=Common.BZECookie;
                options.LoginPath="/Accounts/Login";
                options.AccessDeniedPath = "/Accounts/Login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            });

            services.AddRazorPages();
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
