using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using webserver.Controllers;
using webserver.Models;
using webserver.Data;

using Microsoft.AspNetCore.Mvc;
using webserver.Models.DTOs;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace webserver.Tests.Project.Controllers;
public class ClientControllerTest : IDisposable {
    
    private readonly WebserverContext _context;
    private readonly ClientController _controller;
    private readonly UserManager<Client> userManager;    
    private readonly RoleManager<IdentityRole> roleManager;

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

    public void Dispose() {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async void ReadClients_ReturnsNotFound_NoMatchesFound() {
        // Arrannge
        var newClientDto = new ClientDTO("Photographer","a1b1-c1d1","peter-parker","peter-parker@hotmail.com","9899344788");
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
        newClientDto = new ClientDTO("Engrish Teacher","qwe123-ert345", "yusaku-takahara", "yusaku_takahara@gmail.com","5559870123");
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
        newClientDto = new ClientDTO("Spanish Teacher","asd213-xcv456", "johnny-carlos", "carlos-john@outlook.com","555159753");
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
    
        // Act
        var result = await _controller.ReadClients("brian",0,2,"name");
    
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void ReadClients_ReturnsOK_MatchesFound() {
        // Arrannge
        var newClientDto = new ClientDTO("Federal Judge","a1b1-c1d1","judge-dread","judgedread@hotmail.com","9899344788");
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
        newClientDto = new ClientDTO("Engrish Teacher","qwe123-ert345", "yusaku-takahara", "yusaku_takahara@gmail.com","5559870123");
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
        newClientDto = new ClientDTO("Swordsman","asd213-xcv456", "john-skywaler", "skywaler-john@empire.com","555159753");
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");

        var expectedDto = new ClientDTO("Spanish teacher","6a4s-q87e", "john-carlos", "el_john96@empire.com","5557770123"); //we expect this one
        await userManager.CreateAsync((Client)expectedDto, "#Client1234");
    
        // Act
        int limit = 1;
        var result = await _controller.ReadClients("john",0,limit,"name");
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResult.Value!.ToString()!;
        ClientDTO[] myDto = JsonConvert.DeserializeObject<ClientDTO[]>(valueJson)!;

        Assert.True(myDto.Length == limit);
        Assert.Equal(expectedDto.Id,myDto[0].Id);
        Assert.Equal(expectedDto.Occupation,myDto[0].Occupation);
        Assert.Equal(expectedDto.UserName,myDto[0].UserName);
        Assert.Equal(expectedDto.Email,myDto[0].Email);
        Assert.Equal(expectedDto.PhoneNumber,myDto[0].PhoneNumber);
    }

    [Fact]
    public async void ReadClient_ReturnsOk_WhenUserExists() {
        // Arrange
        var clientId = "newId";
        var clientUser = "username";
        var clientEmail = "myemail123@gmail.com";
        var newClientDto = new ClientDTO("singer","s1n-n1gg3r",clientUser, clientEmail,"9899344788");

        // Act
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
        var result = _controller.ReadClient(clientId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResult.Value!.ToString()!;
        ClientDTO myDto = JsonConvert.DeserializeObject<ClientDTO>(valueJson)!;

        Assert.Equal(clientId, myDto.Id);
        Assert.Equal(clientUser, myDto.UserName);
        Assert.Equal(clientEmail, myDto.Email);
    }

    [Fact]
    public void ReadClient_ReturnsNotFound_WhenUserDoesNotExist() {
        // Arrange
        var nonExistentClientId = "nonExistentUserId";

        // Act
        var result = _controller.ReadClient(nonExistentClientId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void RegisterUser_ReturnsBadRequest_WhenEmailExists() {
        // Arrange
        var newClientDto = new ClientDTO("Accountant","4cc0-c0un-t4nt","toby-ross", "myemail123@gmail.com", "3226-0637");

        var newClient = (Client)newClientDto;
        newClient.Id = "newCompId1234";

        // Act
        await userManager.CreateAsync(newClient, "#Company1234");
        var result = await _controller.CreateClient(newClientDto, "@1234Password");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        //Email already registered!
    }

    [Fact]
    public async void RegisterUser_ReturnsBadRequest_WhenUsername_Exists() {
        // Arrange
        var newClientDto = new ClientDTO("Accountant","4cc0-c0un-t4nt","toby-ross", "myemail123@gmail.com", "3226-0637");
        
        var newClient = (Client)newClientDto;
        newClient.Id = "newCompId1234";
        newClient.Email = "newemail1234@gmail.com";

        // Act
        await userManager.CreateAsync(newClient, "#Company1234");
        var result = await _controller.CreateClient(newClientDto, "@1234Password");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        //UserName already registered!
    }

    [Fact]
    public async void RegisterUser_ReturnsCreatedAtAction_WhenClientDoesntExist() {
        // Arrange
        var newClientDto = new ClientDTO("business-manager","547s-ref6","jordan-belfort", "belford-business@wallstreet.com", "9899344788");

        // Act
        var result = await _controller.CreateClient(newClientDto, "@1234Password");

        // Assert
        var okResult = Assert.IsType<CreatedAtActionResult>(result);
        ClientDTO myDto = JsonConvert.DeserializeObject<ClientDTO>(okResult.Value.ToJson())!;

        Assert.Equal(newClientDto.Id, myDto.Id);
        Assert.Equal(newClientDto.UserName, myDto.UserName);
        Assert.Equal(newClientDto.Email, myDto.Email);
    }

    [Fact]
    public async void UpdateClient_ReturnsOk_ExistingId() {
        // Arrange
        var existingId = "existing-id";
        var oldClientDto = new ClientDTO("business-manager",existingId,"jordan-belfort", "belford-business@wallstreet.com", "9899344788");

        var newClientDto = oldClientDto;
        newClientDto.UserName = "newNameForTheUser1234";
        newClientDto.PhoneNumber = "98988263255";

        // Act
        await userManager.CreateAsync((Client)oldClientDto, "#Company1234");
        var result = await _controller.UpdateClient(newClientDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        string valueJson = okResult.Value!.ToString()!;
        ClientDTO myDto = JsonConvert.DeserializeObject<ClientDTO>(valueJson)!;

        Assert.Equal(myDto.Id, newClientDto.Id);
        Assert.Equal(myDto.Email, newClientDto.Email);
        Assert.Equal(myDto.PhoneNumber, newClientDto.PhoneNumber);
        Assert.Equal(myDto.UserName, newClientDto.UserName);
    }

    [Fact]
    public async void UpdateClient_ReturnsBadRequest_WhenIdDoesntExist() {
        // Arrange
        var nonExistClientDto = new ClientDTO("business-manager","nonexistingId","jordan-belfort", "belford-business@wallstreet.com", "9899344788");
        // Act
        var result = await _controller.UpdateClient(nonExistClientDto);
        // Arrange
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async void DeleteClient_ReturnsBadRequest_WhenIdDoesntExist() {
        // Arrange
        var nonExistingId = "non-existing-id";
        // Act
        var result = await _controller.DeleteClient(nonExistingId);
        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async void DeleteClient_ReturnsNoContent_WhenIdExists() {
        // Arrange
        var existingId = "existing-id";
        var newClientDto = new ClientDTO("Accountant",existingId,"johnny-ross","john-account@business.com", "3226-0637");

        // Act
        await userManager.CreateAsync((Client)newClientDto, "#Client1234");
        var result = await _controller.DeleteClient(existingId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}