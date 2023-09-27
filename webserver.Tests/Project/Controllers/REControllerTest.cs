using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using webserver.Controllers;
using webserver.Models;
using webserver.Data;
using NuGet.Protocol;
using SQLitePCL;

namespace webserver.Tests.Project.Controllers {
    public class REControllerTest {

        [Fact]
        public async Task ReadRealEstates_ReturnsOkResult_WithValidParameters() {

            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;
            
            using (var context = new WebserverContext(options)) {

                Company comp = new Company{UserName="initialcomp",Email="initialcomp@gmail.com"};
                context.Company.Add(comp);

                context.RealEstates.Add(new RealEstate { Id = 2, Address="Hollywood Boulevard", Name = "Property2", Price = 200});
                context.RealEstates.Add(new RealEstate { Id = 3, Address="Sunset Boulevard", Name = "Property3", Price = 99});
                context.RealEstates.Add(new RealEstate { Id = 4, Address="The Bar", Name = "Property4", Price = 100});
                context.RealEstates.Add(new RealEstate { Id = 5, Address="Something", Name = "Property5", Price = 101});
                context.RealEstates.Add(new RealEstate { Id = 6, Address="Anything", Name = "Property6", Price = 102});
                context.RealEstates.Add(new RealEstate { Id = 7, Address="Whatsoever", Name = "Property7", Price = 103});
                context.SaveChanges();
                
                var controller = new RealEstatesController(context);

                // Act
                var result = await controller.ReadRealEstates(minPrice: 50, maxPrice: 150, offset: 1, limit: 3, sort: "price") as OkObjectResult;

                var query = context.RealEstates.AsQueryable();
                query = query.Where(re => re.Price >= 50 && re.Price <= 150);
                query = query.Skip(1).Take(3);
                var realEstates = query.ToList();

                // Assert
                Assert.NotNull(result);
                Assert.NotNull(realEstates);

                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Property4", realEstates[0].Name);
                Assert.True(realEstates.Count == 3);//Assert.Equal(1, realEstates.Count);                
            }
        }

        [Fact]
        public void ReadRealEstate_ReturnsOkResult_WhenRealEstateExists() {

            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            int readOKID = 56;

            using (var context = new WebserverContext(options)) {

                context.RealEstates.Add(new RealEstate { Id = readOKID, Address = "Sesame Street", Name = "Property1", Price = 40, CompanyId="a1b1c1d1" });
                context.SaveChanges();

                var controller = new RealEstatesController(context);

                // Act
                var result = controller.ReadRealEstate(readOKID) as OkObjectResult;
                var realEstate = context.RealEstates.Find(readOKID);

                // Assert
                Assert.NotNull(result);
                Assert.NotNull(realEstate);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Sesame Street",realEstate.Address);
                Assert.Equal("Property1",realEstate.Name);
                Assert.Equal(40,realEstate.Price);
            }
        }

        [Fact]
        public void ReadRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {
            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            int nonExist = 86;

            using (var context = new WebserverContext(options)) {

                var controller = new RealEstatesController(context);

                // Act
                var result = controller.ReadRealEstate(nonExist) as NotFoundResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal(404, result.StatusCode);
            }
        }

        [Fact]
        public void CreateRealEstate_ReturnsOkResult_WhenRealEstateDoesNotExist() {

            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            int createId = 112;

            using (var context = new WebserverContext(options)) {

                Company comp = new Company { UserName="compania", Email="compania@gmail.com" };
                context.Company.Add(comp);
                context.SaveChanges();

                var controller = new RealEstatesController(context);
                var newRealEstate = new RealEstate { Id = createId, Name = "NewProperty", Price = 300, CompanyId = comp.Id };

                // Act
                var result = controller.CreateRealEstate(newRealEstate) as ObjectResult;
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
        }

        [Fact]
        public void CreateRealEstate_ReturnsBadRequest_WhenRealEstateAlreadyExists() {
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

                var controller = new RealEstatesController(context);
                var existingRealEstate = new RealEstate { Id = existsId, Name = "NewProperty", Price = 300, CompanyId = "a1b1c1d1" };

                // Act
                var result = controller.CreateRealEstate(existingRealEstate) as BadRequestObjectResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Real Estate Already Exists!", result.Value);
            }
        }

        [Fact]
        public void CreateRealEstate_ReturnsBadRequest_WhenOwnerCompanyNotExists() {
            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            int ownerExistsId = 173;

            using (var context = new WebserverContext(options)) {

                var controller = new RealEstatesController(context);
                var newRealEstate = new RealEstate { Id = ownerExistsId, Name = "NewProperty", Price = 300, CompanyId="DefinellyNotExists" };

                // Act
                var result = controller.CreateRealEstate(newRealEstate) as BadRequestObjectResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Owner Company does Not Exist!", result.Value);
            }
        }

        [Fact]
        public void UpdateRealEstate_ReturnsOkResult_WhenRealEstateExists() {
            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            int upId = 195;
            
            using (var context = new WebserverContext(options)) {

                var initialRealEstate = new RealEstate { Id = upId, Name = "Property13", Price = 100, Address = "Address13" };
                var controller = new RealEstatesController(context);
                var updatedRealEstate = new RealEstate { Id = upId, Name = "UpdatedProperty", Price = 200, Address = "UpdatedAddress" };

                context.RealEstates.Add(initialRealEstate);
                context.SaveChanges();

                // Act

                var result = controller.UpdateRealEstate(upId, updatedRealEstate) as OkObjectResult;
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
        public void UpdateRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {

            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            int nonId = 228;

            using (var context = new WebserverContext(options)) {
                var controller = new RealEstatesController(context);
                var updatedRealEstate = new RealEstate { Id = nonId, Name = "UpdatedProperty", Price = 200, Address = "UpdatedAddress" };

                // Act
                var result = controller.UpdateRealEstate(nonId, updatedRealEstate) as NotFoundResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal(404, result.StatusCode);
            }
        }

        [Fact]
        public void DeleteRealEstate_ReturnsNoContent_WhenRealEstateExists() {

            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            int existsId = 246;

            using (var context = new WebserverContext(options)) {

                var initialRealEstate = new RealEstate { Id = existsId, Name = "Property13", Price = 100, Address = "Address13" };
                var controller = new RealEstatesController(context);

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
        public void DeleteRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {

            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            int NotExistId = 275;

            using (var context = new WebserverContext(options)) {

                var controller = new RealEstatesController(context);

                // Act
                var result = controller.DeleteRealEstate(NotExistId) as NotFoundResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal(404, result.StatusCode);
            }
        }
    }
}