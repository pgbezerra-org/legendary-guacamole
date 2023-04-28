using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<RazorPagesMovieContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'RazorPagesMovieContext' not found.")));
/*
//string mysqlcon = builder.Configuration.GetConnectionString("mysqlcon");
string mysqlcon = "server=(localdb)\\MSSQLLocalDB;user=root;database=RazorPagesMovieContext-bd990c08-ec81-438a-8d77-80da93634a86;port=3306;password=livro";
MySqlConnection con = new MySqlConnection(mysqlcon);
con.Open();
*/
var app = builder.Build();
/*
//Creates some Movies ("seed data.cs")
using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
