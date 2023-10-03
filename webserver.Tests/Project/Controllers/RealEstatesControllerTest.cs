using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using webserver.Models;
using webserver.Controllers;
using webserver.Data;
using webserver.Models.DTOs;
using Newtonsoft.Json;
using NuGet.Protocol;

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

        RealEstateDTO newRealEstate = new RealEstateDTO(2, "rio state", "copacabana", 11, companyDto.Id);

        // Act
        var result = await _controller.CreateRealEstate(newRealEstate);

        // Assert
        var response = Assert.IsType<OkObjectResult>(result);
        string json = response.Value!.ToString()!;
        RealEstateDTO responseRealEstate = JsonConvert.DeserializeObject<RealEstateDTO>(json)!;

        Assert.Equal(responseRealEstate.Id, newRealEstate.Id);
        Assert.Equal(responseRealEstate.Name, newRealEstate.Name);
        Assert.Equal(responseRealEstate.Address, newRealEstate.Address);
        Assert.Equal(responseRealEstate.Price, newRealEstate.Price);
        Assert.Equal(responseRealEstate.CompanyId, newRealEstate.CompanyId);
    }

    [Fact]
    public async Task CreateRealEstate_ReturnsBadRequest_WhenRealEstateAlreadyExists() {
        // Arrange
        CompanyDTO companyDto = new CompanyDTO("q2w3e4r5", "company", "company123@hotmail.com", "5557890123", "Brazil", "RJ", "RJ");
        RealEstateDTO newRealEstate = new RealEstateDTO(2, "rio state", "copacabana", 11, companyDto.Id);

        _context.Company.Add((Company)companyDto);
        _context.RealEstates.Add((RealEstate)newRealEstate);
        _context.SaveChanges();

        // Act
        var result = await _controller.CreateRealEstate(newRealEstate);

        // Arrange
        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Value, "Real Estate Already Exists!");
    }

    [Fact]
    public async Task CreateRealEstate_ReturnsBadRequest_WhenOwnerCompanyNotExists() {
        // Arrange
        int stateId = 11;
        RealEstateDTO newRealEstate = new RealEstateDTO(stateId, "rio state", "copacabana", 11000, "noCompanyHere");

        // Act
        var result = await _controller.CreateRealEstate(newRealEstate);

        // Arrange
        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Value, "Owner Company does Not Exist!");
    }

    [Fact]
    public async Task ReadRealEstates_ReturnsOkResult_WithValidParameters() {
        Company comp = new Company{Id="c0mp4=ny55", UserName="initialcomp",Email="initialcomp@gmail.com"};
        _context.Company.Add(comp);

        _context.RealEstates.Add(new RealEstate { Id = 2, Address="Hollywood Boulevard", Name = "Property2", Price = 200, CompanyId=comp.Id});
        _context.RealEstates.Add(new RealEstate { Id = 3, Address="Sunset Boulevard", Name = "Property3", Price = 99, CompanyId=comp.Id});
        _context.RealEstates.Add(new RealEstate { Id = 4, Address="The Bar", Name = "Property4", Price = 100, CompanyId=comp.Id});
        _context.RealEstates.Add(new RealEstate { Id = 5, Address="Something", Name = "Property5", Price = 101, CompanyId=comp.Id});
        _context.RealEstates.Add(new RealEstate { Id = 6, Address="Anything", Name = "Property6", Price = 102, CompanyId=comp.Id});
        _context.RealEstates.Add(new RealEstate { Id = 7, Address="Whatsoever", Name = "Property7", Price = 103, CompanyId=comp.Id});
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
        RealEstateDTO[] realEstatesDtoArray = JsonConvert.DeserializeObject<RealEstateDTO[]>(valueJson)!;

        Assert.True(realEstatesDtoArray!.Length == length);
        Assert.Equal("The Bar", realEstates[0].Address);
        Assert.Equal("Property4", realEstates[0].Name);
        Assert.Equal(100, realEstates[0].Price);
    }

    [Fact]
    public async Task ReadRealEstates_ReturnsNotFound_NoMatchesFound() {
        Company comp = new Company{Id="c0mp4=ny55", UserName="initialcomp",Email="initialcomp@gmail.com"};
        _context.Company.Add(comp);

        _context.RealEstates.Add(new RealEstate { Id = 2, Address="Hollywood Boulevard", Name = "Property2", Price = 50, CompanyId = comp.Id});
        _context.RealEstates.Add(new RealEstate { Id = 3, Address="Sunset Boulevard", Name = "Property3", Price = 100, CompanyId = comp.Id});
        _context.RealEstates.Add(new RealEstate { Id = 5, Address="Something", Name = "Property5", Price = 200, CompanyId = comp.Id});
        _context.RealEstates.Add(new RealEstate { Id = 6, Address="Anything", Name = "Property6", Price = 250, CompanyId = comp.Id});
        _context.RealEstates.Add(new RealEstate { Id = 7, Address="Whatsoever", Name = "Property7", Price = 300, CompanyId = comp.Id});
        _context.RealEstates.Add(new RealEstate { Id = 4, Address="The Bar", Name = "AAA", Price = 150, CompanyId = comp.Id});
        _context.SaveChanges();
        
        // Act
        int length = 3;

        var query = _context.RealEstates.AsQueryable();
        query = query.Where(re => re.Price > 50 && re.Price < 300).Skip(1).Take(length).OrderBy(r=>r.Name);

        RealEstate[] realEstates = query.ToArray();
        var response = await _controller.ReadRealEstates(minPrice: 50, maxPrice: 300, offset: 1, limit: length, sort: "name");

        // Assert
        var result = Assert.IsType<OkObjectResult>(response);
        string valueJson = result.Value!.ToString()!;
        RealEstateDTO[] realEstatesDtoArray = JsonConvert.DeserializeObject<RealEstateDTO[]>(valueJson)!;

        Assert.True(realEstatesDtoArray!.Length == length);
        Assert.Equal("The Bar", realEstates[0].Address);
        Assert.Equal("AAA", realEstates[0].Name);
        Assert.Equal(150, realEstates[0].Price);
    }

    [Fact]
    public async Task ReadRealEstate_ReturnsOkResult_WhenRealEstateExists() {
        // Arrange
        int idToRead = 76;
        Company comp = new Company { Id="a1b1c1d1", UserName = "exampleCompany"};
        RealEstateDTO realDto = new RealEstateDTO(idToRead, "Sesame Street", "Sesame House", 40, "a1b1c1d1");

        _context.Company.Add(comp);
        _context.RealEstates.Add((RealEstate)realDto);
        _context.SaveChanges();

        // Act
        var result = await _controller.ReadRealEstate(idToRead);

        // Assert
        var response = Assert.IsType<OkObjectResult>(result);
        string valueJson = response.Value!.ToString()!;
        RealEstateDTO realEstateDto = JsonConvert.DeserializeObject<RealEstateDTO>(valueJson)!;

        Assert.Equal(realDto.Id, realEstateDto.Id);
        Assert.Equal(realDto.Address, realEstateDto.Address);
        Assert.Equal(realDto.Name, realEstateDto.Name);
        Assert.Equal(realDto.Price, realEstateDto.Price);
        Assert.Equal(realDto.CompanyId, realEstateDto.CompanyId);
    }

    [Fact]
    public async Task ReadRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {
        // Act
        var result = await _controller.ReadRealEstate(0);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateRealEstate_ReturnsOkResult_WhenRealEstateExists() {
        // Assert
        CompanyDTO companyDto = new CompanyDTO("q2w3e4r5", "company", "company123@hotmail.com", "5557890123", "Brazil", "RJ", "RJ");
        RealEstateDTO newRealEstate = new RealEstateDTO(2, "rio state", "copacabana", 11, companyDto.Id);

        _context.Company.Add((Company)companyDto);
        _context.RealEstates.Add((RealEstate)newRealEstate);
        _context.SaveChanges();
    
        newRealEstate.Name="Copacabana pallace";
        newRealEstate.Address="Copacabana Beach";
        newRealEstate.Price=11000000;

        // Act
        var result = await _controller.UpdateRealEstate(newRealEstate);

        // Arrange
        var response = Assert.IsType<OkObjectResult>(result);
        var json = response.Value!.ToString()!;
        RealEstateDTO responseDto = JsonConvert.DeserializeObject<RealEstateDTO>(json)!;
        
        Assert.Equal(responseDto.Id, newRealEstate.Id);
        Assert.Equal(responseDto.Name, newRealEstate.Name);
        Assert.Equal(responseDto.Address, newRealEstate.Address);
        Assert.Equal(responseDto.Price, newRealEstate.Price);
        Assert.Equal(responseDto.CompanyId, newRealEstate.CompanyId);
    }

    [Fact]
    public async Task UpdateRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {
        // Assert
        RealEstateDTO fictionDto = new RealEstateDTO(229, "fiction", "neverland", 229000, "nonexist");
    
        // Act
        var result = await _controller.UpdateRealEstate(fictionDto);
    
        // Arrange
        var response = Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteRealEstate_ReturnsNoContent_WhenRealEstateExists() {
        // Arrange
        int idExist = 76;
        RealEstateDTO realDto = new RealEstateDTO(idExist, "Sesame Street", "Sesame House", 40, "a1b1c1d1");
        CompanyDTO companyDto = new CompanyDTO("a1b1c1d1", "company", "company123@hotmail.com", "5557890123", "Brazil", "RJ", "RJ");

        _context.Company.Add((Company)companyDto);
        _context.RealEstates.Add((RealEstate)realDto);
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