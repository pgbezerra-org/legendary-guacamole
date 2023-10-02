using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using webserver.Controllers;
using webserver.Models;
using webserver.Data;
using webserver.Models.DTOs;
using Newtonsoft.Json;
using NuGet.Protocol;
using Microsoft.AspNetCore.Http.HttpResults;

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
    public async void ReadCompanies_ReturnsNotFound_NoMatchesFound() {
        // Arrannge
        var newCompDto = new CompanyDTO("a1b1c1d1","username","myemail123@gmail.com","9899344788","brazil","MA","Sao Luis");
        await userManager.CreateAsync((Company)newCompDto, "#Company1234");
        newCompDto = new CompanyDTO("a1s2d3f4","anothername","another123@gmail.com","98988263255","USA","NY","NY");
        await userManager.CreateAsync((Company)newCompDto, "#Company1234");
    
        // When
        var result = await _controller.ReadCompanies(1,1,"USA","Nebraska","Lincoln",null);
    
        // Then
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void ReadCompanies_ReturnsOK_MatchesFound() {
        // Arrannge
        var newCompDto = new CompanyDTO("a1b1c1d1","username","myemail123@gmail.com","9899344788","brazil","MA","Sao Luis");
        await userManager.CreateAsync((Company)newCompDto, "#Company1234");
        newCompDto = new CompanyDTO("d4s-1st-gu77","deutsch-mensch","deutsch@gmail.com","5559880123","Germany","Berlin","Berlin");
        await userManager.CreateAsync((Company)newCompDto, "#Company1234");
        newCompDto = new CompanyDTO("qwert5678","yeti-another","yeti-man@gmail.com","5559880123","USA","Texas","Dallas");   //Expects only this
        await userManager.CreateAsync((Company)newCompDto, "#Company1234");

        var expectedDto = new CompanyDTO("a1s2d3f4","anothername","another123@gmail.com","98988263255","USA","NY","NY");
        await userManager.CreateAsync((Company)expectedDto, "#Company1234");
    
        // When
        int limit = 1;
        var result = await _controller.ReadCompanies(1,limit,"USA",null,null,"city");
    
        // Then
        var okResult = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResult.Value!.ToString()!;
        CompanyDTO[] myDto = JsonConvert.DeserializeObject<CompanyDTO[]>(valueJson)!;

        Assert.True(myDto.Length == limit);
        Assert.Equal(expectedDto.UserName,myDto[0].UserName);
        Assert.Equal(expectedDto.Email,myDto[0].Email);
        Assert.Equal(expectedDto.PhoneNumber,myDto[0].PhoneNumber);
    }

    [Fact]
    public async void ReadCompany_ReturnsOk_WhenUserExists() {
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
    public async void RegisterUser_ReturnsBadRequest_WhenEmailExists() {
        // Arrange
        var email = "myemail123@gmail.com";
        var newCompDto = new CompanyDTO("newId","username",email, "9899344788","brazil","MA","Sao Luis");

        var newComp = (Company)newCompDto;
        newComp.Id = "newCompId1234";

        // Act
        await userManager.CreateAsync(newComp, "#Company1234");
        var result = await _controller.CreateCompany(newCompDto, "@1234Password");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async void RegisterUser_ReturnsBadRequest_WhenUsername_Exists() {
        // Arrange
        var email = "myemail123@gmail.com";
        var newCompDto = new CompanyDTO("newId","username",email, "9899344788","brazil","MA","Sao Luis");
        
        var newComp = (Company)newCompDto;
        newComp.Id = "newCompId1234";
        newComp.Email = "newemail1234@gmail.com";

        // Act
        await userManager.CreateAsync(newComp, "#Company1234");
        var result = await _controller.CreateCompany(newCompDto, "@1234Password");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async void RegisterUser_ReturnsCreatedAtAction_WhenCompanyDoesntExist() {
        // Arrange
        var newCompDto = new CompanyDTO("newId","username","myemail123@gmail.com", "9899344788","brazil","MA","Sao Luis");

        // Act
        var result = await _controller.CreateCompany(newCompDto, "@1234Password");

        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(result);
        CompanyDTO myDto = JsonConvert.DeserializeObject<CompanyDTO>(okResult.Value.ToJson())!;

        Assert.Equal(newCompDto.Id, myDto.Id);
        Assert.Equal(newCompDto.UserName, myDto.UserName);
        Assert.Equal(newCompDto.Email, myDto.Email);
    }

    [Fact]
    public async void UpdateCompany_ReturnsOk_ExistingId() {
        // Arrange
        var existingId = "existing-id";
        var oldCompDto = new CompanyDTO(existingId,"username","myemail123@gmail.com","9899344788","brazil","MA","Sao Luis");

        var newCompDto = oldCompDto;
        newCompDto.UserName = "newNameForTheUser1234";
        newCompDto.PhoneNumber = "98988263255";

        // Act
        await userManager.CreateAsync((Company)oldCompDto, "#Company1234");
        var result = await _controller.UpdateCompany(newCompDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResult.Value!.ToString()!;
        CompanyDTO myDto = JsonConvert.DeserializeObject<CompanyDTO>(valueJson)!;

        Assert.Equal(myDto.Id, newCompDto.Id);
        Assert.Equal(myDto.Email, newCompDto.Email);
        Assert.Equal(myDto.PhoneNumber, newCompDto.PhoneNumber);
        Assert.Equal(myDto.UserName, newCompDto.UserName);
    }

    [Fact]
    public async void UpdateCompany_ReturnsBadRequest_WhenIdDoesntExist() {
        // Arrange
        CompanyDTO nonExistCompDto = new CompanyDTO("nonExistId","lendacerda","#NonPass7890","32260637","Brazil","MA","Sao Luis");
        // Act
        var result = await _controller.UpdateCompany(nonExistCompDto);
        // Arrange
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async void DeleteCompany_ReturnsBadRequest_WhenIdDoesntExist() {
        // Arrange
        var nonExistingId = "non-existing-id";
        
        // Act
        var result = await _controller.DeleteCompany(nonExistingId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async void DeleteCompany_ReturnsNoContent_WhenIdExists() {
        // Arrange
        var existingId = "existing-id";
        var newCompDto = new CompanyDTO(existingId,"username","myemail123@gmail.com","9899344788","brazil","MA","Sao Luis");

        // Act
        await userManager.CreateAsync((Company)newCompDto, "#Company1234");
        var result = await _controller.DeleteCompany(existingId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }    

    //adicionar a checagem do statusCode
    //add 'useRole' in the api tests
    //not let the user send bad Dtos even tho the responses go as they should
}