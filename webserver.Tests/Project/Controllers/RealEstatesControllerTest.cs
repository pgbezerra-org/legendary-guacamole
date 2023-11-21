using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using webserver.Data;
using webserver.Models;
using webserver.Controllers;
using webserver.Models.DTOs;

namespace webserver.Tests.Project.Controllers;
/// <summary>
/// XUnit tests of the RealEstates ControllerBase class
/// </summary>
public class RealEstatesControllerTest : IDisposable {

    private readonly WebserverContext _context;
    private readonly RealEstatesController _controller;

    /// <summary>
    /// RealEstatesController Tests constructor
    /// Creates a SQLite database for testing
    /// Also manually creates services that would've been created as singletons in the real project,
    /// such as Managers, IdentityClasses and DbCOntext
    /// </summary>
    public RealEstatesControllerTest(){
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

        _controller = new RealEstatesController(_context);
    }

    /// <summary>
    /// Method that MUST be called to free resources when finishing using IDisposable classes
    /// </summary>
    public void Dispose() {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    /// <summary>
    /// Tests the Create method, expects a Successfull result
    /// </summary>
    [Fact]
    public async Task CreateRealEstate_ReturnsOkResult_WhenRealEstateDoesNotExist() {
        // Arrange
        CompanyDTO companyDto = new CompanyDTO("q2w3e4r5", "company", "company123@hotmail.com", "5557890123", "Brazil", "RJ", "RJ");
        _context.Company.Add((Company)companyDto);
        _context.SaveChanges();

        RealEstate newRealEstate = new RealEstate("rio state", "copacabana", 11, companyDto.Id);

        // Act
        var result = await _controller.CreateRealEstate(newRealEstate);

        // Assert
        var response = Assert.IsType<OkObjectResult>(result);
        string json = response.Value!.ToString()!;
        RealEstate responseRealEstate = JsonConvert.DeserializeObject<RealEstate>(json)!;

        Assert.Equal(responseRealEstate.Name, newRealEstate.Name);
        Assert.Equal(responseRealEstate.Address, newRealEstate.Address);
        Assert.Equal(responseRealEstate.Price, newRealEstate.Price);
        Assert.Equal(responseRealEstate.CompanyId, newRealEstate.CompanyId);
    }

    /// <summary>
    /// Tests the Create method, expects a Bad Request response since the Real Estate must be Owned by a Registered Company
    /// </summary>
    [Fact]
    public async Task CreateRealEstate_ReturnsBadRequest_WhenOwnerCompanyNotExists() {
        // Arrange
        RealEstate newRealEstate = new RealEstate("rio state", "copacabana", 11000, "noCompanyHere");
        // Act
        var result = await _controller.CreateRealEstate(newRealEstate);
        // Arrange
        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Value, "Owner Company does Not Exist!");
    }

    /// <summary>
    /// Tests the ReadRealEstates method, expects a result with specific element as it's first element
    /// </summary>
    [Fact]
    public async Task ReadRealEstates_ReturnsOkResult_WithValidParameters() {
        //Arrange
        Company comp = new Company("c0mp4=ny55","initialcomp","initialcomp@gmail.com","9832263255","Brazil","RJ","RJ");
        _context.Company.Add(comp);

        RealEstate targetEstate = new RealEstate("Cavalier", "Property4", 200, comp.Id);

        _context.RealEstates.Add(new RealEstate("Abacuque", "Property2", 100, comp.Id));
        _context.RealEstates.Add(new RealEstate("Boulevard", "Property3", 150, comp.Id));
        _context.RealEstates.Add(targetEstate);
        _context.RealEstates.Add(new RealEstate("Delta", "Property5", 250, comp.Id));
        _context.RealEstates.Add(new RealEstate("Echo echo", "Property6", 300, comp.Id));
        _context.RealEstates.Add(new RealEstate("Forgery", "Property7", 350, comp.Id));

        _context.SaveChanges();
        
        // Act
        int length = 3;

        var query = _context.RealEstates.AsQueryable();
        query = query.Where(re => re.Price >= 150 && re.Price <= 350).Skip(1).Take(length);

        RealEstate[] realEstates = query.ToArray();
        var response = await _controller.ReadRealEstates(minPrice: 101, maxPrice: 350, offset: 1, limit: length, sort: "address");

        // Assert
        var result = Assert.IsType<OkObjectResult>(response);
        string valueJson = result.Value!.ToString()!;
        RealEstate[] realEstatesArray = JsonConvert.DeserializeObject<RealEstate[]>(valueJson)!;

        Assert.True(realEstatesArray!.Length == length);
        Assert.Equal(targetEstate.Name, realEstates[0].Name);
        Assert.Equal(targetEstate.Address, realEstates[0].Address);        
        Assert.Equal(targetEstate.Price, realEstates[0].Price);
        Assert.Equal(targetEstate.CompanyId, realEstates[0].CompanyId);
    }

    /// <summary>
    /// Tests the ReadRealEstates method, expects a NotFound return since no results are expected to meet the criteria
    /// </summary>
    [Fact]
    public async Task ReadRealEstates_ReturnsNotFound_NoMatchesFound() {
        Company comp = new Company("c0mp4=ny55","initialcomp","initialcomp@gmail.com","9832263255","Brazil","RJ","RJ");
        _context.Company.Add(comp);

        _context.RealEstates.Add(new RealEstate("Hollywood Boulevard", "Property2", 50, comp.Id));
        _context.RealEstates.Add(new RealEstate("Sunset Boulevard", "Property3", 100, comp.Id));
        _context.RealEstates.Add(new RealEstate("The Bar", "AAA", 150, comp.Id));
        _context.RealEstates.Add(new RealEstate("Something", "Property5", 200, comp.Id));
        _context.RealEstates.Add(new RealEstate("Anything", "Property6", 250, comp.Id));
        _context.RealEstates.Add(new RealEstate("Whatsoever", "Property7", 300, comp.Id));
        _context.SaveChanges();
        
        // Act
        int length = 3;
        int minPrice = 50, maxPrice = 150;

        var query = _context.RealEstates.AsQueryable();
        query = query.Where(re => re.Price > 50 && re.Price < 150).Skip(1).Take(length).OrderBy(r=>r.Name);
        RealEstate[] realEstates = query.ToArray();

        var response = await _controller.ReadRealEstates(minPrice + 1, maxPrice - 1, offset: 1, limit: length, sort: "name");

        // Assert
        var result = Assert.IsType<NotFoundResult>(response);
    }

    /// <summary>
    /// Tests the ReadBZEmployee method, expects a specific result
    /// </summary>
    [Fact]
    public async Task ReadRealEstate_ReturnsOkResult_WhenRealEstateExists() {
        // Arrange
        int idToRead = 142;

        Company comp = new Company("a1b1c1d1","exampleCompany","comp123@gmail.com","9832263255","Brazil","RJ","RJ");
        RealEstate realEstateToRead = new RealEstate("Sesame Street", "Sesame House", 40, "a1b1c1d1");
        realEstateToRead.Id = idToRead;

        _context.Company.Add(comp);
        _context.RealEstates.Add(realEstateToRead);
        _context.SaveChanges();

        // Act
        var result = await _controller.ReadRealEstate(idToRead);

        // Assert
        var response = Assert.IsType<OkObjectResult>(result);
        string valueJson = response.Value!.ToString()!;
        RealEstate responseRealEstate = JsonConvert.DeserializeObject<RealEstate>(valueJson)!;

        Assert.Equal(realEstateToRead.Id, responseRealEstate.Id);
        Assert.Equal(realEstateToRead.Address, responseRealEstate.Address);
        Assert.Equal(realEstateToRead.Name, responseRealEstate.Name);
        Assert.Equal(realEstateToRead.Price, responseRealEstate.Price);
        Assert.Equal(realEstateToRead.CompanyId, responseRealEstate.CompanyId);
    }

    /// <summary>
    /// Tests the Update method, expects a Bad Request since the Id is not found
    /// </summary>
    [Fact]
    public async Task ReadRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {
        // Act
        var result = await _controller.ReadRealEstate(155);
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the Update method, expects a Succesfull response with the updated Real Estate's data
    /// </summary>
    [Fact]
    public async Task UpdateRealEstate_ReturnsOkResult_WhenRealEstateExists() {
        // Assert
        CompanyDTO companyDto = new CompanyDTO("q2w3e4r5", "company", "company123@hotmail.com", "5557890123", "Brazil", "RJ", "RJ");
        RealEstate newRealEstate = new RealEstate("rio state", "copacabana", 11, companyDto.Id);

        _context.Company.Add((Company)companyDto);
        _context.RealEstates.Add(newRealEstate);
        _context.SaveChanges();
    
        newRealEstate.Name = "Copacabana pallace";
        newRealEstate.Address = "Copacabana Beach";
        newRealEstate.Price = 11000000;

        // Act
        var result = await _controller.UpdateRealEstate(newRealEstate);

        // Arrange
        var response = Assert.IsType<OkObjectResult>(result);
        var json = response.Value!.ToString()!;
        RealEstate responseRealEstate = JsonConvert.DeserializeObject<RealEstate>(json)!;
        
        Assert.Equal(responseRealEstate.Id, newRealEstate.Id);
        Assert.Equal(responseRealEstate.Name, newRealEstate.Name);
        Assert.Equal(responseRealEstate.Address, newRealEstate.Address);
        Assert.Equal(responseRealEstate.Price, newRealEstate.Price);
        Assert.Equal(responseRealEstate.CompanyId, newRealEstate.CompanyId);
    }

    /// <summary>
    /// Tests the Update method, expects NotFoundResult since there are no users in the database
    /// </summary>
    [Fact]
    public async Task UpdateRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {
        // Assert
        RealEstate nonExistRealEstate = new RealEstate("fiction", "neverland", 229000, "nonexist");
        // Act
        var result = await _controller.UpdateRealEstate(nonExistRealEstate);    
        // Arrange
        var response = Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Tests the Delete method, expects a Bad Request since the Id isn't there
    /// </summary>
    [Fact]
    public async Task DeleteRealEstate_ReturnsNoContent_WhenRealEstateExists() {
        // Arrange
        int idExist = 2;
        CompanyDTO companyDto = new CompanyDTO("a1b1c1d1", "company", "company123@hotmail.com", "5557890123", "Brazil", "RJ", "RJ");

        RealEstate realEstate = new RealEstate("Sesame Street", "Sesame House", 40, "a1b1c1d1");
        realEstate.Id = idExist;

        _context.Company.Add((Company)companyDto);
        _context.RealEstates.Add(realEstate);
        _context.SaveChanges();

        // Act
        var result = await _controller.DeleteRealEstate(idExist);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Tests the Delete method, expects a successful response, meaning NoContentResult
    /// </summary>
    [Fact]
    public async Task DeleteRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {
        // Act
        var result = await _controller.DeleteRealEstate(76);
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}