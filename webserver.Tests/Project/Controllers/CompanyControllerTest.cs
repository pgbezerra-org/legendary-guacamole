using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using webserver.Data;
using webserver.Models;
using webserver.Models.DTOs;
using webserver.Controllers;

namespace webserver.Tests.Project.Controllers;
/// <summary>
/// XUnit tests of the Company ControllerBase class
/// </summary>
public class CompanyControllerTest : IDisposable {
    private readonly WebserverContext _context;
    private readonly CompanyController _controller;
    private readonly UserManager<Company> userManager;    
    private readonly RoleManager<IdentityRole> roleManager;

    /// <summary>
    /// BZE_Controller Tests constructor
    /// Creates a SQLite database for testing
    /// Also manually creates services that would've been created as singletons in the real project,
    /// such as Managers, IdentityClasses and DbCOntext
    /// </summary>
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
        var userValidator = new UserValidator<Company>();
        var passwordHasher = new PasswordHasher<Company>();
        var userValidators = new List<IUserValidator<Company>> { userValidator };

        var identityOptions = new IdentityOptions();
        IOptions<IdentityOptions> optionsParameter = Options.Create(identityOptions);

        var passwordValidators = new List<IPasswordValidator<Company>> {
            new PasswordValidator<Company>(),
        };

        userManager = new UserManager<Company>( userStore, optionsParameter, passwordHasher, userValidators, passwordValidators,
            new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), Moq.Mock.Of<IServiceProvider>(), new Logger<UserManager<Company>>(new LoggerFactory()));

        var roleStore = new RoleStore<IdentityRole>(_context);
        var roleValidator = new List<IRoleValidator<IdentityRole>> { new RoleValidator<IdentityRole>() };        
        roleManager = new RoleManager<IdentityRole>( roleStore, roleValidator,
            new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), new Logger<RoleManager<IdentityRole>>(new LoggerFactory()));

        _controller = new CompanyController(_context, userManager, roleManager);        
    }

    /// <summary>
    /// Method that MUST be called to free resources when finishing using IDisposable classes
    /// </summary>
    public void Dispose() {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    /// <summary>
    /// Tests the ReadCompanies method, expects a NotFound return since no results are expected to meet the criteria
    /// </summary>
    [Fact]
    public async void ReadCompanies_ReturnsNotFound_NoMatchesFound() {
        // Arrannge
        var newCompDto = new CompanyDTO("a1b1c1d1","username","myemail123@gmail.com","9899344788","brazil","MA","Sao Luis");
        await userManager.CreateAsync((Company)newCompDto, "#Company1234");
        newCompDto = new CompanyDTO("a1s2d3f4","anothername","another123@gmail.com","98988263255","USA","NY","NY");
        await userManager.CreateAsync((Company)newCompDto, "#Company1234");
    
        // Act
        var result = await _controller.ReadCompanies(1,1,"USA","Nebraska","Lincoln",null);
    
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the ReadCompanies method, expects a result with specific element as it's first element
    /// </summary>
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
    
        // Act
        int limit = 1;
        var result = await _controller.ReadCompanies(1,limit,"USA",null,null,"city");
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResult.Value!.ToString()!;
        CompanyDTO[] myDto = JsonConvert.DeserializeObject<CompanyDTO[]>(valueJson)!;

        Assert.True(myDto.Length == limit);
        Assert.Equal(expectedDto.UserName,myDto[0].UserName);
        Assert.Equal(expectedDto.Email,myDto[0].Email);
        Assert.Equal(expectedDto.PhoneNumber,myDto[0].PhoneNumber);
    }

    /// <summary>
    /// Tests the Read method for a specific Company, expects a specific returned Client
    /// </summary>
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

        Assert.Equal(companyUser, myDto.UserName);
        Assert.Equal(companyEmail, myDto.Email);
    }

    /// <summary>
    /// Tests the ReadBZEmployees method, expects no result since there are no users in the database
    /// </summary>
    [Fact]
    public void ReadCompany_ReturnsNotFound_WhenUserDoesNotExist() {
        // Arrange
        var nonExistentCompanyId = "nonExistentUserId";

        // Act
        var result = _controller.ReadCompany(nonExistentCompanyId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the Create method, expects a Bad Request response since the Email is already registered
    /// </summary>
    [Fact]
    public async void CreateCompany_ReturnsBadRequest_WhenEmailExists() {
        // Arrange
        var email = "myemail123@gmail.com";
        var newCompDto = new CompanyDTO("newId","username",email, "9899344788","brazil","MA","Sao Luis");

        var newComp = (Company)newCompDto;

        // Act
        await userManager.CreateAsync(newComp, "#Company1234");
        var result = await _controller.CreateCompany(newCompDto, "@1234Password");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Tests the Create method, expects a Bad Request response since the UserName is already registered
    /// </summary>
    [Fact]
    public async void CreateCompany_ReturnsBadRequest_WhenUsername_Exists() {
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

    /// <summary>
    /// Tests the Create method, expects a Successfull result
    /// </summary>
    [Fact]
    public async void CreateCompany_ReturnsCreatedAtAction_WhenCompanyDoesntExist() {
        // Arrange
        var newCompDto = new CompanyDTO("newId","username","myemail123@gmail.com", "9899344788","brazil","MA","Sao Luis");

        // Act
        var result = await _controller.CreateCompany(newCompDto, "@1234Password");

        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(result);
        CompanyDTO myDto = JsonConvert.DeserializeObject<CompanyDTO>(okResult.Value.ToJson())!;

        Assert.Equal(newCompDto.UserName, myDto.UserName);
        Assert.Equal(newCompDto.Email, myDto.Email);
    }

    /// <summary>
    /// Tests the Update method, expects a Succesfull response with the updated user's data
    /// </summary>
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

        Assert.Equal(myDto.Email, newCompDto.Email);
        Assert.Equal(myDto.PhoneNumber, newCompDto.PhoneNumber);
        Assert.Equal(myDto.UserName, newCompDto.UserName);
    }

    /// <summary>
    /// Tests the Update method, expects NotFoundResult since there are no users in the database
    /// </summary>
    [Fact]
    public async void UpdateCompany_ReturnsBadRequest_WhenIdDoesntExist() {
        // Arrange
        CompanyDTO nonExistCompDto = new CompanyDTO("nonExistId","lendacerda","#NonPass7890","32260637","Brazil","MA","Sao Luis");
        // Act
        var result = await _controller.UpdateCompany(nonExistCompDto);
        // Arrange
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Tests the Delete method, expects a Bad Request since the Id isn't there
    /// </summary>
    [Fact]
    public async void DeleteCompany_ReturnsBadRequest_WhenIdDoesntExist() {
        // Arrange
        var nonExistingId = "non-existing-id";
        
        // Act
        var result = await _controller.DeleteCompany(nonExistingId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Tests the Delete method, expects a successful response, meaning NoContentResult
    /// </summary>
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
}