using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using webserver.Models;
using webserver.Controllers;
using webserver.Data;
using webserver.Models.DTOs;
using Newtonsoft.Json;

namespace webserver.Tests.Project.Controllers;
public class RealEstatesControllerTest : IDisposable {

    private readonly WebserverContext _context;
    private readonly RealEstatesController _controller;

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

    public void Dispose() {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task CreateRealEstate_ReturnsOkResult_WhenRealEstateDoesNotExist() {
        // Arrange
        CompanyDTO companyDto = new CompanyDTO("q2w3e4r5", "company", "company123@hotmail.com", "5557890123", "Brazil", "RJ", "RJ");
        _context.Company.Add((Company)companyDto);
        _context.SaveChanges();

        RealEstate newRealEstate = new RealEstate(2, "rio state", "copacabana", 11, companyDto.Id);

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

    [Fact]
    public async Task CreateRealEstate_ReturnsBadRequest_WhenOwnerCompanyNotExists() {
        // Arrange
        RealEstate newRealEstate = new RealEstate(0, "rio state", "copacabana", 11000, "noCompanyHere");
        // Act
        var result = await _controller.CreateRealEstate(newRealEstate);
        // Arrange
        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Value, "Owner Company does Not Exist!");
    }

    [Fact]
    public async Task ReadRealEstates_ReturnsOkResult_WithValidParameters() {
        //Arrange
        Company comp = new Company("c0mp4=ny55","initialcomp","initialcomp@gmail.com","9832263255","Brazil","RJ","RJ");
        _context.Company.Add(comp);

        _context.RealEstates.Add(new RealEstate ( 2, "Hollywood Boulevard", "Property2", 200, comp.Id));
        _context.RealEstates.Add(new RealEstate ( 3, "Sunset Boulevard", "Property3", 99, comp.Id));
        _context.RealEstates.Add(new RealEstate ( 5, "Something", "Property5", 101, comp.Id));
        _context.RealEstates.Add(new RealEstate ( 6, "Anything", "Property6", 102, comp.Id));
        _context.RealEstates.Add(new RealEstate ( 7, "Whatsoever", "Property7", 103, comp.Id));

        RealEstate targetEstate = new RealEstate ( 4, "The Bar", "Property4", 100, comp.Id);
        _context.RealEstates.Add(targetEstate);

        _context.SaveChanges();
        
        // Act
        int length = 3;

        var query = _context.RealEstates.AsQueryable();
        query = query.Where(re => re.Price >= 50 && re.Price <= 150).Skip(1).Take(length);

        RealEstate[] realEstates = query.ToArray();
        var response = await _controller.ReadRealEstates(minPrice: 50, maxPrice: 150, offset: 1, limit: length, sort: "price");

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

    [Fact]
    public async Task ReadRealEstates_ReturnsNotFound_NoMatchesFound() {
        Company comp = new Company("c0mp4=ny55","initialcomp","initialcomp@gmail.com","9832263255","Brazil","RJ","RJ");
        _context.Company.Add(comp);

        _context.RealEstates.Add(new RealEstate (2, "Hollywood Boulevard", "Property2", 50, comp.Id));
        _context.RealEstates.Add(new RealEstate (3, "Sunset Boulevard", "Property3", 100, comp.Id));
        _context.RealEstates.Add(new RealEstate (4, "The Bar", "AAA", 150, comp.Id));
        _context.RealEstates.Add(new RealEstate (5, "Something", "Property5", 200, comp.Id));
        _context.RealEstates.Add(new RealEstate (6, "Anything", "Property6", 250, comp.Id));
        _context.RealEstates.Add(new RealEstate (7, "Whatsoever", "Property7", 300, comp.Id));
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

    [Fact]
    public async Task ReadRealEstate_ReturnsOkResult_WhenRealEstateExists() {
        // Arrange
        Company comp = new Company("a1b1c1d1","exampleCompany","comp123@gmail.com","9832263255","Brazil","RJ","RJ");
        RealEstate realEstateToRead = new RealEstate(1, "Sesame Street", "Sesame House", 40, "a1b1c1d1");

        _context.Company.Add(comp);
        int idToRead = _context.RealEstates.Add(realEstateToRead).Entity.Id;
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

    [Fact]
    public async Task ReadRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {
        // Act
        var result = await _controller.ReadRealEstate(155);
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateRealEstate_ReturnsOkResult_WhenRealEstateExists() {
        // Assert
        CompanyDTO companyDto = new CompanyDTO("q2w3e4r5", "company", "company123@hotmail.com", "5557890123", "Brazil", "RJ", "RJ");
        RealEstate newRealEstate = new RealEstate(2, "rio state", "copacabana", 11, companyDto.Id);

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

    [Fact]
    public async Task UpdateRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {
        // Assert
        RealEstate nonExistRealEstate = new RealEstate(229, "fiction", "neverland", 229000, "nonexist");
        // Act
        var result = await _controller.UpdateRealEstate(nonExistRealEstate);    
        // Arrange
        var response = Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteRealEstate_ReturnsNoContent_WhenRealEstateExists() {
        // Arrange
        RealEstate realEstate = new RealEstate(213, "Sesame Street", "Sesame House", 40, "a1b1c1d1");
        CompanyDTO companyDto = new CompanyDTO("a1b1c1d1", "company", "company123@hotmail.com", "5557890123", "Brazil", "RJ", "RJ");

        _context.Company.Add((Company)companyDto);
        int idExist = _context.RealEstates.Add(realEstate).Entity.Id;
        _context.SaveChanges();

        // Act
        var result = await _controller.DeleteRealEstate(idExist);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {
        // Act
        var result = await _controller.DeleteRealEstate(76);
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}