using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using webserver.Data;
using webserver.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add database context
builder.Services.AddDbContext<webserverContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'RazorPagesMovieContext' not found.")));

builder.Services.AddIdentity<Company, IdentityRole>()
    .AddEntityFrameworkStores<webserverContext>()
    .AddDefaultTokenProviders();

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
/*
builder.Services.ConfigureApplicationCookie(options=>{
                options.Cookie.Name=Common.BZE_Cookie;
                options.LoginPath="/Accounts/Login";
                options.AccessDeniedPath = "/Accounts/Login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            });*/

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