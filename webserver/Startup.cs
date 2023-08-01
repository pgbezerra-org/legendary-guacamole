using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using webserver.Data;
using webserver.Models;

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

            /*services.AddDbContext<webserverContext>(options =>
    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));*/

            services.AddDbContext<WebserverContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("MyConnection")));

            services.AddAuthentication().AddCookie(Common.BZECookie, options => {
                options.Cookie.Name = Common.BZECookie;
            });

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

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
                endpoints.MapControllers();
            });
        }

    }
}
