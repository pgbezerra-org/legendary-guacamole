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
/// XUnit tests of the BZEmployee ControllerBase class
/// </summary>
public class BZEmployeeControllerTest : IDisposable {
    
    private readonly WebserverContext _context;
    private readonly BZEmployeeController _controller;
    private readonly UserManager<BZEmployee> userManager;    
    private readonly RoleManager<IdentityRole> roleManager;

    /// <summary>
    /// BZE_Controller Tests constructor
    /// Creates a SQLite database for testing
    /// Also manually creates services that would've been created as singletons in the real project,
    /// such as Managers, IdentityClasses and DbCOntext
    /// </summary>
    public BZEmployeeControllerTest(){
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

        var userStore = new UserStore<BZEmployee>(_context);
        var userValidator = new UserValidator<BZEmployee>();
        var passwordHasher = new PasswordHasher<BZEmployee>();
        var userValidators = new List<IUserValidator<BZEmployee>> { userValidator };

        var identityOptions = new IdentityOptions();
        IOptions<IdentityOptions> optionsParameter = Options.Create(identityOptions);

        var passwordValidators = new List<IPasswordValidator<BZEmployee>> {
            new PasswordValidator<BZEmployee>(),
        };

        userManager = new UserManager<BZEmployee>( userStore, optionsParameter, passwordHasher, userValidators, passwordValidators,
            new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), Moq.Mock.Of<IServiceProvider>(), new Logger<UserManager<BZEmployee>>(new LoggerFactory()));

        var roleStore = new RoleStore<IdentityRole>(_context);
        var roleValidator = new List<IRoleValidator<IdentityRole>> { new RoleValidator<IdentityRole>() };        
        roleManager = new RoleManager<IdentityRole>( roleStore, roleValidator,
            new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), new Logger<RoleManager<IdentityRole>>(new LoggerFactory()));

        _controller = new BZEmployeeController(_context, userManager, roleManager);
    }

    /// <summary>
    /// Method that MUST be called to free resources when finishing using IDisposable classes
    /// </summary>
    public void Dispose() {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    /// <summary>
    /// Tests the ReadBZEmployees method, expects a NotFound return since no results are expected to meet the criteria
    /// </summary>
    [Fact]
    public async void ReadBZEmployees_ReturnsNotFound_NoMatchesFound() {
        // Arrannge
        var newBZEmployeeDto = new BZEmployeeDTO("a1b1-c1d1","peter-parker","peter-parker@hotmail.com","9899344788", 6800);
        await userManager.CreateAsync((BZEmployee)newBZEmployeeDto, "#BZEmployee1234");
        newBZEmployeeDto = new BZEmployeeDTO("qwe123-ert345", "yusaku-takahara", "yusaku_takahara@gmail.com","5559870123", 7100);
        await userManager.CreateAsync((BZEmployee)newBZEmployeeDto, "#BZEmployee1234");
        newBZEmployeeDto = new BZEmployeeDTO("asd213-xcv456", "johnny-carlos", "carlos-john@outlook.com","555159753", 7300);
        await userManager.CreateAsync((BZEmployee)newBZEmployeeDto, "#BZEmployee1234");
    
        // Act
        var result = await _controller.ReadBZEmployees("brian",0,2,"name");
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the ReadBZEmployees method, expects a result with specific element as it's first element
    /// </summary>
    [Fact]
    public async void ReadBZEmployees_ReturnsOK_MatchesFound() {
        // Arrannge
        var newBZEmployeeDto = new BZEmployeeDTO("a1b1-c1d1","judge-dread","judgedread@hotmail.com","9899344788", 8500);
        await userManager.CreateAsync((BZEmployee)newBZEmployeeDto, "#BZEmployee1234");
        newBZEmployeeDto = new BZEmployeeDTO("qwe123-ert345", "yusaku-takahara", "yusaku_takahara@gmail.com","5559870123", 8700);
        await userManager.CreateAsync((BZEmployee)newBZEmployeeDto, "#BZEmployee1234");
        newBZEmployeeDto = new BZEmployeeDTO("asd213-xcv456", "john-skywaler", "skywaler-john@empire.com","555159753", 8900);
        await userManager.CreateAsync((BZEmployee)newBZEmployeeDto, "#BZEmployee1234");

        var expectedDto = new BZEmployeeDTO("6a4s-q87e", "john-carlos", "el_john96@empire.com","5557770123", 9100); //we expect this one
        await userManager.CreateAsync((BZEmployee)expectedDto, "#BZEmployee1234");
    
        // Act
        int limit = 1;
        var result = await _controller.ReadBZEmployees("john",0,limit,"name");
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResult.Value!.ToString()!;
        BZEmployeeDTO[] responseDto = JsonConvert.DeserializeObject<BZEmployeeDTO[]>(valueJson)!;

        Assert.True(responseDto.Length == limit);
        Assert.Equal(expectedDto.Salary,responseDto[0].Salary);
        Assert.Equal(expectedDto.UserName,responseDto[0].UserName);
        Assert.Equal(expectedDto.Email,responseDto[0].Email);
        Assert.Equal(expectedDto.PhoneNumber,responseDto[0].PhoneNumber);
    }

    /// <summary>
    /// Tests the ReadBZEmployee method, expects a specific result
    /// </summary>
    [Fact]
    public async void ReadBZEmployee_ReturnsOk_WhenUserExists() {
        // Arrange
        var BZEmployeeId = "newId";
        var newBZEmployeeDto = new BZEmployeeDTO(BZEmployeeId, "myusername123", "myemail123@gmail.com", "9899344788", 2500);

        // Act
        await userManager.CreateAsync((BZEmployee)newBZEmployeeDto, "#BZEmployee1234");
        var result = _controller.ReadBZEmployee(BZEmployeeId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResult.Value!.ToString()!;
        BZEmployeeDTO responseDto = JsonConvert.DeserializeObject<BZEmployeeDTO>(valueJson)!;

        Assert.Equal(newBZEmployeeDto.UserName, responseDto.UserName);
        Assert.Equal(newBZEmployeeDto.Email, responseDto.Email);
        Assert.Equal(newBZEmployeeDto.PhoneNumber, responseDto.PhoneNumber);
        Assert.Equal(newBZEmployeeDto.Salary, responseDto.Salary);
    }

    /// <summary>
    /// Tests the ReadBZEmployees method, expects no result since there are no users in the database
    /// </summary>
    [Fact]
    public void ReadBZEmployee_ReturnsNotFound_WhenUserDoesNotExist() {
        // Arrange
        var nonExistentBZEmployeeId = "nonExistentUserId";
        // Act
        var result = _controller.ReadBZEmployee(nonExistentBZEmployeeId);
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the Create method, expects a Bad Request response since the Email is already registered
    /// </summary>
    [Fact]
    public async void CreateBZEmployee_ReturnsBadRequest_WhenEmailExists() {
        // Arrange
        var newBZEmployeeDto = new BZEmployeeDTO("4cc0-c0un-t4nt","toby-ross", "myemail123@gmail.com", "3226-0637", 2500);

        var newBZEmployee = (BZEmployee)newBZEmployeeDto;
        newBZEmployee.UserName = "anotherUser";

        // Act
        await userManager.CreateAsync(newBZEmployee, "#BZEmployee1234");
        var result = await _controller.CreateBZEmployee(newBZEmployeeDto, "@1234Password");

        // Assert
        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Value, "Email already registered!");
    }

    /// <summary>
    /// Tests the Create method, expects a Bad Request response since the UserName is already registered
    /// </summary>
    [Fact]
    public async void CreateBZEmployee_ReturnsBadRequest_WhenUsernameExists() {

        // Arrange
        var newBZEmployeeDto = new BZEmployeeDTO("4cc0-c0un-t4nt","toby-ross", "myemail123@gmail.com", "3226-0637", 2500);
        
        var newBZEmployee = (BZEmployee)newBZEmployeeDto;
        newBZEmployee.Email = "newemail1234@gmail.com";

        // Act
        await userManager.CreateAsync(newBZEmployee, "#BZEmployee1234");
        var result = await _controller.CreateBZEmployee(newBZEmployeeDto, "@1234Password");

        // Assert
        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Value, "UserName already registered!");
    }

    /// <summary>
    /// Tests the Create method, expects a Successfull result
    /// </summary>
    [Fact]
    public async void CreateBZEmployee_ReturnsCreatedAtAction_WhenBZEmployeeDoesntExist() {
        // Arrange
        var newBZEmployeeDto = new BZEmployeeDTO("547s-ref6","jordan-belfort", "belford-business@wallstreet.com", "9899344788", 2500);

        // Act
        var result = await _controller.CreateBZEmployee(newBZEmployeeDto, "@1234Password");

        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(result);
        BZEmployeeDTO responseDto = JsonConvert.DeserializeObject<BZEmployeeDTO>(okResult.Value.ToJson())!;

        Assert.Equal(newBZEmployeeDto.UserName, responseDto.UserName);
        Assert.Equal(newBZEmployeeDto.Email, responseDto.Email);
        Assert.Equal(newBZEmployeeDto.PhoneNumber, responseDto.PhoneNumber);
        Assert.Equal(newBZEmployeeDto.Salary, responseDto.Salary);
    }

    /// <summary>
    /// Tests the Update method, expects NotFoundResult since there are no users in the database
    /// </summary>
    [Fact]
    public async void UpdateBZEmployee_ReturnsBadRequest_WhenIdDoesntExist() {
        // Arrange
        var nonExistBZEmployeeDto = new BZEmployeeDTO("nonexistingId","jordan-belfort", "belford-business@wallstreet.com", "9899344788", 2500);
        // Act
        var result = await _controller.UpdateBZEmployee(nonExistBZEmployeeDto);
        // Arrange
        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Value, "BZEmployee does not Exist!");
    }

    /// <summary>
    /// Tests the Update method, expects a Succesfull response with the updated user's data
    /// </summary>
    [Fact]
    public async void UpdateBZEmployee_ReturnsOk_ExistingId() {
        // Arrange
        var existingId = "existing-id";
        var oldBZEmployeeDto = new BZEmployeeDTO(existingId,"jordan-belfort", "belford-business@wallstreet.com", "9899344788", 2500);

        var newBZEmployeeDto = oldBZEmployeeDto;
        newBZEmployeeDto.UserName = "newNameForTheUser1234";
        newBZEmployeeDto.PhoneNumber = "98988263255";

        // Act
        await userManager.CreateAsync((BZEmployee)oldBZEmployeeDto, "#BZEmployee1234");
        var result = await _controller.UpdateBZEmployee(newBZEmployeeDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResult.Value!.ToString()!;
        BZEmployeeDTO resultDto = JsonConvert.DeserializeObject<BZEmployeeDTO>(valueJson)!;

        Assert.Equal(resultDto.Email, newBZEmployeeDto.Email);
        Assert.Equal(resultDto.PhoneNumber, newBZEmployeeDto.PhoneNumber);
        Assert.Equal(resultDto.UserName, newBZEmployeeDto.UserName);
        Assert.Equal(resultDto.Salary, newBZEmployeeDto.Salary);
    }

    /// <summary>
    /// Tests the Delete method, expects a Bad Request since the Id isn't there
    /// </summary>
    [Fact]
    public async void DeleteBZEmployee_ReturnsBadRequest_WhenIdDoesntExist() {
        // Arrange
        var nonExistingId = "non-existing-id";
        // Act
        var result = await _controller.DeleteBZEmployee(nonExistingId);
        // Assert
        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Value, "BZEmployee does not Exist!");
    }

    /// <summary>
    /// Tests the Delete method, expects a successful response, meaning NoContentResult
    /// </summary>
    [Fact]
    public async void DeleteBZEmployee_ReturnsNoContent_WhenIdExists() {
        // Arrange
        var existingId = "existing-id";
        var newBZEmployeeDto = new BZEmployeeDTO(existingId,"johnny-ross","john-account@business.com", "3226-0637", 2500);

        // Act
        await userManager.CreateAsync((BZEmployee)newBZEmployeeDto, "#BZEmployee1234");
        var result = await _controller.DeleteBZEmployee(existingId);
        var deletedBZEmp = _context.BZEmployees.Find(existingId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Null(deletedBZEmp);
    }
}