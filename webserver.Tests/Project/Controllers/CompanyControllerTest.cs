using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using webserver.Controllers;
using webserver.Models;
using webserver.Data;
using webserver.Models.DTOs;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;

namespace webserver.Tests.Project.Controllers;

public class CompanyControllerTest : IDisposable {
    private readonly WebserverContext _context;

    private readonly CompanyController _controller;

    private readonly UserManager<Company> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public CompanyControllerTest() {
        var connectionStringBuilder = new SqliteConnectionStringBuilder {
            DataSource = ":memory:"
        };
        var connection = new SqliteConnection(connectionStringBuilder.ToString());

        
        DbContextOptions<WebserverContext> _options = new DbContextOptionsBuilder<WebserverContext>()
            .UseSqlite(connection)
            .Options;

        _context = new WebserverContext(_options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        var userStore = new UserStore<Company>(_context);
        var passwordValidator = new PasswordValidator<Company>();
        var userValidator = new UserValidator<Company>();

        // Create an instance of IPasswordHasher<Company> and IUserValidator<Company>
        var passwordHasher = new PasswordHasher<Company>();
        var userValidators = new List<IUserValidator<Company>> { userValidator };

        userManager = new UserManager<Company>( userStore, null, passwordHasher, userValidators,
            null, null, null, null, null);
        
        var roleStore = new RoleStore<IdentityRole>(_context);
        var roleValidator = new List<IRoleValidator<IdentityRole>> { new RoleValidator<IdentityRole>() };
        roleManager = new RoleManager<IdentityRole>(roleStore, roleValidator, null, null, null);

        _controller=new CompanyController(_context, userManager, roleManager);        
    }

    public void Dispose() {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void ReadCompany_ReturnsOk_WhenUserExists() {
        // Arrange
        var companyId = "existingUserId";
        var companyEmail = "existemail@gmail.com";
        var company = new Company { Id = companyId, Email = companyEmail };
        
        // Act
        _context.Company.Add(company);
        _context.SaveChanges();
        var result = _controller.ReadCompany(companyId);

        // Assert
        var okResponse = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResponse.Value!.ToString()!;
        CompanyDTO myDto = JsonConvert.DeserializeObject<CompanyDTO>(valueJson)!;

        Assert.Equal(companyId, myDto.Id);
        Assert.Equal(companyEmail, myDto.Email);
    }

    [Fact]
    public void ReadCompany_ReturnsNotFound_WhenUserDoesNotExist() {
        
        // Arrange
        var nonExistentCompanyId = "nonExistentUserId";

        // Act
        var result = _controller.ReadCompany(nonExistentCompanyId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    
}