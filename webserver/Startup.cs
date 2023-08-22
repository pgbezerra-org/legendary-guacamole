using Microsoft.AspNetCore.Identity;
using webserver.Data;
using webserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace webserver {

    public class Startup {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {

            services.AddIdentity<Company, IdentityRole>()
    .AddEntityFrameworkStores<webserverContext>()
    .AddDefaultTokenProviders();

            services.AddMvc();

            services.AddDbContext<webserverContext>(options =>
    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication().AddCookie("MyCookieAuth", options => {
                options.Cookie.Name = "MyCookieAuth";
            });

            services.AddRazorPages();
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider) {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
            
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { Common.BZE_Role, Common.Company_Role, Common.Client_Role };
            foreach (var roleName in roleNames) {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist){
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            if (env.IsDevelopment()) {
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

    }
}
