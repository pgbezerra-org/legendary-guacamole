using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using webserver.Data;
using webserver.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add database context
builder.Services.AddDbContext<webserverContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'Guacamole' not found.")));

builder.Services.AddDbContext<webserverContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MyConnection") ?? throw new InvalidOperationException("Connection string 'Guacamole' not found."), new MySqlServerVersion(new Version(8, 0, 26))));


builder.Services.AddIdentity<Company, IdentityRole>()
    .AddEntityFrameworkStores<webserverContext>()
    .AddDefaultTokenProviders();

// Add authentication
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options => {
        options.Cookie.Name = "MyCookieAuth";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Events.OnRedirectToAccessDenied = context => {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToLogin = context => {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();