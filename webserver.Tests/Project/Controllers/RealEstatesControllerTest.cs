using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using webserver.Models;
using webserver.Data;
using webserver.Models.DTOs;

namespace webserver.Tests.Project.Controllers {
    public class RealEstatesController {

    [Fact]
    public async Task ReadRealEstates_ReturnsOkResult_WithValidParameters() {

        Company comp = new Company{UserName="initialcomp",Email="initialcomp@gmail.com"};
        _context.Company.Add(comp);

        _context.RealEstates.Add(new RealEstate { Id = 2, Address="Hollywood Boulevard", Name = "Property2", Price = 200});
        _context.RealEstates.Add(new RealEstate { Id = 3, Address="Sunset Boulevard", Name = "Property3", Price = 99});
        _context.RealEstates.Add(new RealEstate { Id = 4, Address="The Bar", Name = "Property4", Price = 100});
        _context.RealEstates.Add(new RealEstate { Id = 5, Address="Something", Name = "Property5", Price = 101});
        _context.RealEstates.Add(new RealEstate { Id = 6, Address="Anything", Name = "Property6", Price = 102});
        _context.RealEstates.Add(new RealEstate { Id = 7, Address="Whatsoever", Name = "Property7", Price = 103});
        _context.SaveChanges();
        
        // Act
        int length = 3;

        var query = _context.RealEstates.AsQueryable();
        query = query.Where(re => re.Price >= 50 && re.Price <= 150).Skip(1).Take(length);

        var realEstates = query.ToArray();
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
    public async Task ReadRealEstate_ReturnsOkResult_WhenRealEstateExists() {

        // Arrange
        int idToRead = 76;
        RealEstateDTO realDto = new RealEstateDTO(idToRead, "Sesame Street", "Sesame House", 40, "a1b1c1d1");

        _context.RealEstates.Add((RealEstate)realDto);
        _context.SaveChanges();

        // Act
        var result = await _controller.ReadRealEstate(idToRead);

        // Assert
        var response = Assert.IsType<OkObjectResult>(result);
        string valueJson = response.Value!.ToString()!;
        RealEstateDTO[] realEstatesDtoArray = JsonConvert.DeserializeObject<RealEstateDTO[]>(valueJson)!;

        Assert.Equal(realDto.Id, realEstatesDtoArray[0].Id);
        Assert.Equal(realDto.Address, realEstatesDtoArray[0].Address);
        Assert.Equal(realDto.Name, realEstatesDtoArray[0].Name);
        Assert.Equal(realDto.Price, realEstatesDtoArray[0].Price);
        Assert.Equal(realDto.CompanyId, realEstatesDtoArray[0].CompanyId);
    }

    [Fact]
    public async Task ReadRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {
        // Act
        var result = _controller.ReadRealEstate(0);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateRealEstate_ReturnsOkResult_WhenRealEstateDoesNotExist() {

        // Arrange
        Company comp = new Company { UserName="compania", Email="compania@gmail.com" };
        _context.Company.Add(comp);
        _context.SaveChanges();

        var newRealEstate = new RealEstate { Id = createId, Name = "NewProperty", Price = 300, CompanyId = comp.Id };

        // Act
        var result = controller.CreateRealEstate((RealEstateDTO)newRealEstate) as ObjectResult;
        var createdRealEstate = context.RealEstates.Find(createId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(createdRealEstate);
        Assert.Equal(newRealEstate.Id, createdRealEstate.Id);
        Assert.Equal(newRealEstate.Name, createdRealEstate.Name);
        Assert.Equal(newRealEstate.Price, createdRealEstate.Price);
        Assert.Equal(newRealEstate.Address, createdRealEstate.Address);
        Assert.Equal(newRealEstate.CompanyId, createdRealEstate.CompanyId);
    }

    [Fact]
    public async Task CreateRealEstate_ReturnsBadRequest_WhenRealEstateAlreadyExists() {
        // Arrange
        var options = new DbContextOptionsBuilder<WebserverContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;
        
        int existsId = 141;

        using (var context = new WebserverContext(options)) {

            Company comp = new Company { UserName = "TotallyExists", Email = "totalexist@gmail.com"};
            context.Company.Add(comp);
            context.RealEstates.Add(new RealEstate { Id = existsId, Name = "ExistingProperty", Price = 200, CompanyId = comp.Id });
            context.SaveChanges();

            var controller = new webserver.Controllers.RealEstatesController(context);
            var existingRealEstate = new RealEstate { Id = existsId, Name = "NewProperty", Price = 300, CompanyId = "a1b1c1d1" };

            // Act
            var result = controller.CreateRealEstate((RealEstateDTO)existingRealEstate) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Real Estate Already Exists!", result.Value);
        }
    }

    [Fact]
    public async Task CreateRealEstate_ReturnsBadRequest_WhenOwnerCompanyNotExists() {
        // Arrange
        var options = new DbContextOptionsBuilder<WebserverContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        int ownerExistsId = 173;

        using (var context = new WebserverContext(options)) {

            var controller = new webserver.Controllers.RealEstatesController(context);
            var newRealEstate = new RealEstate { Id = ownerExistsId, Name = "NewProperty", Price = 300, CompanyId="DefinellyNotExists" };

            // Act
            var result = controller.CreateRealEstate((RealEstateDTO)newRealEstate) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Owner Company does Not Exist!", result.Value);
        }
    }

    [Fact]
    public async Task UpdateRealEstate_ReturnsOkResult_WhenRealEstateExists() {
        // Arrange
        var options = new DbContextOptionsBuilder<WebserverContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        int upId = 195;
        
        using (var context = new WebserverContext(options)) {

            var initialRealEstate = new RealEstate { Id = upId, Name = "Property13", Price = 100, Address = "Address13" };
            var controller = new webserver.Controllers.RealEstatesController(context);
            var updatedRealEstate = new RealEstate { Id = upId, Name = "UpdatedProperty", Price = 200, Address = "UpdatedAddress" };

            context.RealEstates.Add(initialRealEstate);
            context.SaveChanges();

            // Act

            var result = controller.UpdateRealEstate((RealEstateDTO)updatedRealEstate) as OkObjectResult;
            var updatedResult = context.RealEstates.Find(upId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(updatedResult);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(updatedRealEstate.Name, updatedResult.Name);
            Assert.Equal(updatedRealEstate.Price, updatedResult.Price);
            Assert.Equal(updatedRealEstate.Address, updatedResult.Address);
            Assert.Equal(updatedRealEstate.CompanyId, updatedResult.CompanyId);
        }
    }

    [Fact]
    public async Task UpdateRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {

        // Arrange
        var options = new DbContextOptionsBuilder<WebserverContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        int nonId = 228;

        using (var context = new WebserverContext(options)) {
            var controller = new webserver.Controllers.RealEstatesController(context);
            var updatedRealEstate = new RealEstate { Id = nonId, Name = "UpdatedProperty", Price = 200, Address = "UpdatedAddress" };

            // Act
            var result = controller.UpdateRealEstate((RealEstateDTO)updatedRealEstate) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }
    }

    [Fact]
    public async Task DeleteRealEstate_ReturnsNoContent_WhenRealEstateExists() {

        // Arrange
        var options = new DbContextOptionsBuilder<WebserverContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        int existsId = 246;

        using (var context = new WebserverContext(options)) {

            var initialRealEstate = new RealEstate { Id = existsId, Name = "Property13", Price = 100, Address = "Address13" };
            var controller = new webserver.Controllers.RealEstatesController(context);

            context.RealEstates.Add(initialRealEstate);
            context.SaveChanges();

            // Act
            var result = controller.DeleteRealEstate(existsId) as NoContentResult;
            var deletedRealEstate = context.RealEstates.Find(existsId);

            // Assert
            Assert.NotNull(result);
            Assert.Null(deletedRealEstate);
            Assert.Equal(204, result.StatusCode);                
        }
    }

    [Fact]
    public async Task DeleteRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {

        // Arrange
        var options = new DbContextOptionsBuilder<WebserverContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        int NotExistId = 275;

        using (var context = new WebserverContext(options)) {

            var controller = new webserver.Controllers.RealEstatesController(context);

            // Act
            var result = controller.DeleteRealEstate(NotExistId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }
    }
}