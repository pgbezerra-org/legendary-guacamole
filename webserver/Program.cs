using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webserver.Data;
using webserver.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<WebserverContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("MyConnection") ?? throw new InvalidOperationException("Connection string 'Guacamole' not found.")));

builder.Services.AddDefaultIdentity<BZEAccount>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<WebserverContext>()
        .AddDefaultTokenProviders();

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
    c.SwaggerDoc("api", new OpenApiInfo { Title = "GuacAPI", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    // Add XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Add security definitions and requirements if needed
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>()
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllers();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/api/swagger.json", "GuacAPI");
    c.RoutePrefix = "swagger";
    c.DocExpansion(DocExpansion.None);
});

app.Run();