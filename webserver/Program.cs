using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webserver.Data;
using webserver.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<WebserverContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("MyConnection") ?? throw new InvalidOperationException("Connection string 'Guacamole' not found.")));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter(); //useless but alright

builder.Services.AddDefaultIdentity<BZEAccount>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<WebserverContext>()
        .AddDefaultTokenProviders();

//builder.Services.AddDefaultIdentity<BZEmployee>().AddEntityFrameworkStores<WebserverContext>();
builder.Services.AddIdentityCore<BZEmployee>().AddRoles<IdentityRole>().AddEntityFrameworkStores<WebserverContext>();
builder.Services.AddIdentityCore<Company>().AddRoles<IdentityRole>().AddEntityFrameworkStores<WebserverContext>();
builder.Services.AddIdentityCore<Client>().AddRoles<IdentityRole>().AddEntityFrameworkStores<WebserverContext>();

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath="/Accounts/Login";
        options.AccessDeniedPath = "/Accounts/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.Name = Common.BZE_Cookie;
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GuacAPI", Version = "v1" });
    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllers();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern:"{controller=Home}/{action=Index}/{id?}"
);

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GuacAPI");
    c.RoutePrefix = "swagger";
    c.DocExpansion(DocExpansion.None);
});

app.Run();