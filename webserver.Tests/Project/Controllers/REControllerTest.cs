using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using webserver.Models;
using webserver.Data;
using webserver.Controllers;

namespace webserver.Tests.Project.Controllers {
    public class REControllerTest {

        [Fact]
        public async Task ReadRealEstates_ReturnsOkResult_WithValidParameters() {
            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;
            
            using (var context = new WebserverContext(options)) {
                //context.Company.Add(new Company { Id = "a1b1c1d1", UserName = "ExampleCompany", Email = "exempComp@gmail.com" });
                context.RealEstates.Add(new RealEstate { Id = 1, Address="Sesame Street", Name = "Property1", Price = 40, CompanyId="a1b1c1d1" });
                context.RealEstates.Add(new RealEstate { Id = 2, Address="Hollywood Boulevard", Name = "Property2", Price = 200, CompanyId = "a1b1c1d1" });
                context.RealEstates.Add(new RealEstate { Id = 3, Address="Sunset Boulevard", Name = "Property3", Price = 99, CompanyId = "a1b1c1d1" });
                context.RealEstates.Add(new RealEstate { Id = 4, Address="The Bar", Name = "Property4", Price = 100, CompanyId = "a1b1c1d1" });
                context.RealEstates.Add(new RealEstate { Id = 5, Address="Something", Name = "Property5", Price = 101, CompanyId = "a1b1c1d1" });
                context.RealEstates.Add(new RealEstate { Id = 6, Address="Anything", Name = "Property6", Price = 102, CompanyId = "a1b1c1d1" });
                context.RealEstates.Add(new RealEstate { Id = 7, Address="Whatsoever", Name = "Property7", Price = 103, CompanyId = "a1b1c1d1" });
                context.SaveChanges();
                
                var controller = new RealEstatesController(context);

                // Act
                var result = await controller.ReadRealEstates(minPrice: 50, maxPrice: 150, offset: 1, limit: 3, sort: "price") as OkObjectResult;
                var realEstates = result.Value  as RealEstate[];
                //var realEstates = JsonConvert.DeserializeObject<RealEstate[]>(result.Value.ToJToken().ToArray());

                // Assert
                Assert.NotNull(result);
                Assert.NotNull(realEstates);

                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Property4", realEstates[0].Name);
                Assert.True(realEstates.Length == 3);//Assert.Equal(1, realEstates.Count);                
            }
        }

        [Fact]
        public void ReadRealEstate_ReturnsOkResult_WhenRealEstateExists() {
            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            using (var context = new WebserverContext(options)) {
                context.RealEstates.Add(new RealEstate { Id = 1, Address="Sesame Street", Name = "Property1", Price = 40, CompanyId="a1b1c1d1" });
                context.SaveChanges();
                var controller = new RealEstatesController(context);

                // Act
                var result = controller.ReadRealEstate(1) as OkObjectResult;
                var realEstate = result.Value as RealEstate;

                // Assert
                Assert.NotNull(result);
                Assert.NotNull(realEstate);
                Assert.Equal(200, result.StatusCode);
            }
        }

        [Fact]
        public void ReadRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {
            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            int nonExist = 404;

            using (var context = new WebserverContext(options)) {
                context.RealEstates.Add(new RealEstate { Id = nonExist, Address="Sesame Street", Name = "Property1", Price = 40, CompanyId="a1b1c1d1" });
                context.SaveChanges();
                var controller = new RealEstatesController(context);

                // Act
                var result = controller.ReadRealEstate(nonExist) as NotFoundResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal(404, result.StatusCode);
            }
        }

        [Fact]
        public void ReadRealEstate_ReturnsNotFound_WhenOwnerCompanyDoesNotExist() {
            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            using (var context = new WebserverContext(options)) {
                context.RealEstates.Add(new RealEstate { Id = 1, Address="Sesame Street", Name = "Property1", Price = 40, CompanyId="DefinellyNotExist" });
                context.SaveChanges();
                var controller = new RealEstatesController(context);

                // Act
                var result = controller.ReadRealEstate(1) as NotFoundResult;

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

            using (var context = new WebserverContext(options)) {
                var controller = new RealEstatesController(context);
                var newRealEstate = new RealEstate { Id = 11, Name = "NewProperty", Price = 300, CompanyId = "a1b1c1d1" };

                // Act
                var actionResult = controller.CreateRealEstate(newRealEstate) as CreatedAtActionResult;
                var createdRealEstate = (RealEstate)actionResult.Value;

                // Assert
                Assert.NotNull(actionResult);
                Assert.NotNull(createdRealEstate);
                Assert.Equal(201, actionResult.StatusCode);
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

            using (var context = new WebserverContext(options)) {
                // Adding a real estate with the same Id
                context.RealEstates.Add(new RealEstate { Id = 1, Name = "ExistingProperty", Price = 200, CompanyId = "a1b1c1d1" });
                context.SaveChanges();

                var controller = new RealEstatesController(context);
                var existingRealEstate = new RealEstate { Id = 1, Name = "NewProperty", Price = 300, CompanyId = "a1b1c1d1" };

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

            using (var context = new WebserverContext(options)) {
                var controller = new RealEstatesController(context);
                var newRealEstate = new RealEstate { Id = 14, Name = "NewProperty", Price = 300, CompanyId="DefinellyNotExists" };

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

            int upId=13;
            
            using (var context = new WebserverContext(options)) {

                var initialRealEstate = new RealEstate { Id = upId, Name = "Property13", Price = 100, Address = "Address13" };
                context.RealEstates.Add(initialRealEstate);
                context.SaveChanges();

                var controller = new RealEstatesController(context);
                var updatedRealEstate = new RealEstate { Id = upId, Name = "UpdatedProperty", Price = 200, Address = "UpdatedAddress" };

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

            int nonId=999;

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

            int testId=1;

            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            using (var context = new WebserverContext(options)) {

                var controller = new RealEstatesController(context);

                // Act
                var result = controller.DeleteRealEstate(testId) as NoContentResult;
                var deletedRealEstate = context.RealEstates.Find(testId);

                // Assert
                Assert.NotNull(result);
                Assert.Null(deletedRealEstate);
                Assert.Equal(204, result.StatusCode);                
            }
        }

        [Fact]
        public void DeleteRealEstate_ReturnsNotFound_WhenRealEstateDoesNotExist() {

            // Arrange
            int NotExistId=404;
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

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