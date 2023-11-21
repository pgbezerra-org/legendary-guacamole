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
/// XUnit tests of the Client ControllerBase class
/// </summary>
public class ClientControllerTest : IDisposable {
    
    private readonly WebserverContext _context;
    private readonly ClientController _controller;
    private readonly UserManager<Client> userManager;    
    private readonly RoleManager<IdentityRole> roleManager;

    /// <summary>
    /// BZE_Controller Tests constructor
    /// Creates a SQLite database for testing
    /// Also manually creates services that would've been created as singletons in the real project,
    /// such as Managers, IdentityClasses and DbCOntext
    /// </summary>
    public ClientControllerTest(){
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

        var userStore = new UserStore<Client>(_context);
        var userValidator = new UserValidator<Client>();
        var passwordHasher = new PasswordHasher<Client>();
        var userValidators = new List<IUserValidator<Client>> { userValidator };

        var identityOptions = new IdentityOptions();
        IOptions<IdentityOptions> optionsParameter = Options.Create(identityOptions);

        var passwordValidators = new List<IPasswordValidator<Client>> {
            new PasswordValidator<Client>(),
        };

        userManager = new UserManager<Client>( userStore, optionsParameter, passwordHasher, userValidators, passwordValidators,
            new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), Moq.Mock.Of<IServiceProvider>(), new Logger<UserManager<Client>>(new LoggerFactory()));

        var roleStore = new RoleStore<IdentityRole>(_context);
        var roleValidator = new List<IRoleValidator<IdentityRole>> { new RoleValidator<IdentityRole>() };        
        roleManager = new RoleManager<IdentityRole>( roleStore, roleValidator,
            new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), new Logger<RoleManager<IdentityRole>>(new LoggerFactory()));

        _controller = new ClientController(_context, userManager, roleManager);
    }

    /// <summary>
    /// Method that MUST be called to free resources when finishing using IDisposable classes
    /// </summary>
    public void Dispose() {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    /// <summary>
    /// Tests the ReadClients method, expects a NotFound return since no results are expected to meet the criteria
    /// </summary>
    [Fact]
    public async void ReadClients_ReturnsNotFound_NoMatchesFound() {
        // Arrannge
        var newClientDto = new ClientDTO("a1b1-c1d1","peter-parker","peter-parker@hotmail.com","9899344788", "Photographer");
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
        newClientDto = new ClientDTO("qwe123-ert345", "yusaku-takahara", "yusaku_takahara@gmail.com","5559870123", "Engrish Teacher");
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
        newClientDto = new ClientDTO("asd213-xcv456", "johnny-carlos", "carlos-john@outlook.com","555159753", "Spanish Teacher");
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
    
        // Act
        var result = await _controller.ReadClients("brian",0,2,"name");
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the ReadClients method, expects a result with specific element as it's first element
    /// </summary>
    [Fact]
    public async void ReadClients_ReturnsOK_MatchesFound() {
        // Arrannge
        var newClientDto = new ClientDTO("a1b1-c1d1","judge-dread","judgedread@hotmail.com","9899344788", "Federal Judge");
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
        newClientDto = new ClientDTO("qwe123-ert345", "yusaku-takahara", "yusaku_takahara@gmail.com","5559870123", "Engrish Teacher");
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
        newClientDto = new ClientDTO("asd213-xcv456", "john-skywaler", "skywaler-john@empire.com","555159753", "Swordsman");
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");

        var expectedDto = new ClientDTO("6a4s-q87e", "john-carlos", "el_john96@empire.com","5557770123", "Spanish teacher"); //we expect this one
        await userManager.CreateAsync((Client)expectedDto, "#Client1234");
    
        // Act
        int limit = 1;
        var result = await _controller.ReadClients("john",0,limit,"name");
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResult.Value!.ToString()!;
        ClientDTO[] responseDto = JsonConvert.DeserializeObject<ClientDTO[]>(valueJson)!;

        Assert.True(responseDto.Length == limit);
        Assert.Equal(expectedDto.Id,responseDto[0].Id);
        Assert.Equal(expectedDto.Occupation,responseDto[0].Occupation);
        Assert.Equal(expectedDto.UserName,responseDto[0].UserName);
        Assert.Equal(expectedDto.Email,responseDto[0].Email);
        Assert.Equal(expectedDto.PhoneNumber,responseDto[0].PhoneNumber);
    }

    /// <summary>
    /// Tests the ReadBZEmployees method, expects no result since there are no users in the database
    /// </summary>
    [Fact]
    public void ReadClient_ReturnsNotFound_WhenUserDoesNotExist() {
        // Arrange
        var nonExistentClientId = "nonExistentUserId";
        // Act
        var result = _controller.ReadClient(nonExistentClientId);
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the Read method for a specific Client, expects a specific returned Client
    /// </summary>
    [Fact]
    public async void ReadClient_ReturnsOk_WhenUserExists() {
        // Arrange
        var clientId = "newId";
        var newClientDto = new ClientDTO(clientId, "username", "myemail123@gmail.com", "9899344788", "singer");

        // Act
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
        var result = _controller.ReadClient(clientId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResult.Value!.ToString()!;
        ClientDTO responseDto = JsonConvert.DeserializeObject<ClientDTO>(valueJson)!;

        Assert.Equal(clientId, responseDto.Id);
        Assert.Equal(newClientDto.UserName, responseDto.UserName);
        Assert.Equal(newClientDto.Email, responseDto.Email);
        Assert.Equal(newClientDto.PhoneNumber, responseDto.PhoneNumber);
        Assert.Equal(newClientDto.Occupation, responseDto.Occupation);
    }

    /// <summary>
    /// Tests the Create method, expects a Bad Request response since the Email is already registered
    /// </summary>
    [Fact]
    public async void CreateClient_ReturnsBadRequest_WhenEmailExists() {
        // Arrange
        var newClientDto = new ClientDTO("4cc0-c0un-t4nt","toby-ross", "myemail123@gmail.com", "3226-0637", "Accountant");

        var newClient = (Client)newClientDto;
        newClient.Id = "newClientId1234";
        newClient.UserName = "newUsername";

        // Act
        await userManager.CreateAsync(newClient, "#Client1234");
        var result = await _controller.CreateClient(newClientDto, "@1234Password");

        // Assert
        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Value, "Email already registered!");
    }

    /// <summary>
    /// Tests the Create method, expects a Bad Request response since the UserName is already registered
    /// </summary>
    [Fact]
    public async void CreateClient_ReturnsBadRequest_WhenUsername_Exists() {
        // Arrange
        var newClientDto = new ClientDTO("4cc0-c0un-t4nt","toby-ross", "myemail123@gmail.com", "3226-0637", "Accountant");
        
        var newClient = (Client)newClientDto;
        newClient.Id = "newClientId1234";
        newClient.Email = "newemail1234@gmail.com";

        // Act
        await userManager.CreateAsync(newClient, "#Client1234");
        var result = await _controller.CreateClient(newClientDto, "@1234Password");

        // Assert
        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Value, "UserName already registered!");
    }

    /// <summary>
    /// Tests the Create method, expects a Successfull result
    /// </summary>
    [Fact]
    public async void CreateClient_ReturnsCreatedAtAction_WhenClientDoesntExist() {
        // Arrange
        var newClientDto = new ClientDTO("547s-ref6","jordan-belfort", "belford-business@wallstreet.com", "9899344788", "business-manager");
        // Act
        var result = await _controller.CreateClient(newClientDto, "@1234Password");
        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(result);
        ClientDTO myDto = JsonConvert.DeserializeObject<ClientDTO>(okResult.Value.ToJson())!;

        Assert.Equal(newClientDto.Id, myDto.Id);
        Assert.Equal(newClientDto.UserName, myDto.UserName);
        Assert.Equal(newClientDto.Email, myDto.Email);
        Assert.Equal(newClientDto.PhoneNumber, myDto.PhoneNumber);
        Assert.Equal(newClientDto.Occupation, myDto.Occupation);
    }

    /// <summary>
    /// Tests the Update method, expects NotFoundResult since there are no users in the database
    /// </summary>
    [Fact]
    public async void UpdateClient_ReturnsBadRequest_WhenIdDoesntExist() {
        // Arrange
        var nonExistClientDto = new ClientDTO("nonexistingId","jordan-belfort", "belford-business@wallstreet.com", "9899344788", "business manager");
        // Act
        var result = await _controller.UpdateClient(nonExistClientDto);
        // Arrange
        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Value, "Client does not Exist!");
    }

    /// <summary>
    /// Tests the Update method, expects a Succesfull response with the updated user's data
    /// </summary>
    [Fact]
    public async void UpdateClient_ReturnsOk_ExistingId() {
        // Arrange
        var existingId = "existing-id";
        var oldClientDto = new ClientDTO(existingId,"jordan-belfort", "belford-business@wallstreet.com", "9899344788", "business manager");

        var newClientDto = oldClientDto;
        newClientDto.UserName = "newNameForTheUser1234";
        newClientDto.PhoneNumber = "98988263255";
        newClientDto.Occupation = "regional manager";

        // Act
        await userManager.CreateAsync((Client)oldClientDto, "#Client1234");
        var result = await _controller.UpdateClient(newClientDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResult.Value!.ToString()!;
        ClientDTO respondeDto = JsonConvert.DeserializeObject<ClientDTO>(valueJson)!;

        Assert.Equal(respondeDto.Id, newClientDto.Id);
        Assert.Equal(respondeDto.UserName, newClientDto.UserName);
        Assert.Equal(respondeDto.Email, newClientDto.Email);
        Assert.Equal(respondeDto.PhoneNumber, newClientDto.PhoneNumber);
        Assert.Equal(respondeDto.Occupation, newClientDto.Occupation);
    }

    /// <summary>
    /// Tests the Delete method, expects a Bad Request since the Id isn't there
    /// </summary>
    [Fact]
    public async void DeleteClient_ReturnsBadRequest_WhenIdDoesntExist() {
        // Arrange
        var nonExistingId = "non-existing-id";
        // Act
        var result = await _controller.DeleteClient(nonExistingId);
        // Assert
        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Value, "Client does not Exist!");
    }

    /// <summary>
    /// Tests the Delete method, expects a successful response, meaning NoContentResult
    /// </summary>
    [Fact]
    public async void DeleteClient_ReturnsNoContent_WhenIdExists() {
        // Arrange
        var existingId = "existing-id";
        var newClientDto = new ClientDTO(existingId,"johnny-ross","john-account@business.com", "3226-0637", "Accountant");

        // Act
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
        var result = await _controller.DeleteClient(existingId);
        var deleteClient = _context.Clients.Find(existingId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Null(deleteClient);
    }
}