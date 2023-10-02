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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Protocol;

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

        _controller = new CompanyController(_context, userManager, roleManager);        
    }

    public void Dispose() {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task ReadCompany_ReturnsOk_WhenUserExists() {
        // Arrange
        var companyId = "newId";
        var companyUser = "username";
        var companyEmail = "myemail123@gmail.com";
        var newCompDto = new CompanyDTO("newId",companyUser,companyEmail, "9899344788","brazil","MA","Sao Luis");

        // Act
        await userManager.CreateAsync((Company)newCompDto, "#Company1234");
        var result = _controller.ReadCompany(companyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResult.Value!.ToString()!;
        CompanyDTO myDto = JsonConvert.DeserializeObject<CompanyDTO>(valueJson)!;

        Assert.Equal(companyId, myDto.Id);
        Assert.Equal(companyUser, myDto.UserName);
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

    [Fact]
    public async Task RegisterUser_ExistingEmail_ReturnsBadRequest() {
        
        var email = "myemail123@gmail.com";
        var newCompDto = new CompanyDTO("newId","username",email, "9899344788","brazil","MA","Sao Luis");

        var newComp = (Company)newCompDto;
        newComp.Id = "newCompId1234";

        //Act
        await userManager.CreateAsync(newComp, "#Company1234");
        var result = await _controller.CreateCompany(newCompDto, "@1234Password");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RegisterUser_ExistingUsername_ReturnsBadRequest() {
        
        var email = "myemail123@gmail.com";
        var newCompDto = new CompanyDTO("newId","username",email, "9899344788","brazil","MA","Sao Luis");

        var newComp = (Company)newCompDto;
        newComp.Id = "newCompId1234";
        newComp.Email = "newemail1234@gmail.com";

        //Act
        await userManager.CreateAsync(newComp, "#Company1234");
        var result = await _controller.CreateCompany(newCompDto, "@1234Password");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RegisterUser_NewCompany_ReturnsCreatedAtAction() {
        
        var newCompDto = new CompanyDTO("newId","username","myemail123@gmail.com", "9899344788","brazil","MA","Sao Luis");

        //Act
        var result = await _controller.CreateCompany(newCompDto, "@1234Password");

        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(result);
        var resultValue = Assert.IsType<CompanyDTO>(okResult.Value);
        CompanyDTO myDto = JsonConvert.DeserializeObject<CompanyDTO>(resultValue.ToJson())!;

        Assert.Equal(newCompDto.Id, myDto.Id);
        Assert.Equal(newCompDto.UserName, myDto.UserName);
        Assert.Equal(newCompDto.Email, myDto.Email);
    }

    
}