using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using webserver.Models;
using webserver.Data;
using webserver.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace webserver.Tests.Project.Controllers {
    public class REControllerTest {

        [Fact]
        public async Task ReadRealEstate_ReturnsOkResult_WithValidParameters() {
            // Arrange
            var options = new DbContextOptionsBuilder<WebserverContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            // Create some sample data
            using (var context = new WebserverContext(options)) {
                context.RealEstates.Add(new RealEstate { Id = 1, Address="Sesame Street", Name = "Property1", Price = 40 });
                context.RealEstates.Add(new RealEstate { Id = 2, Address="Hollywood Boulevard", Name = "Property2", Price = 200 });
                context.RealEstates.Add(new RealEstate { Id = 3, Address="Sunset Boulevard", Name = "Property3", Price = 99 });
                context.RealEstates.Add(new RealEstate { Id = 4, Address="The Bar", Name = "Property4", Price = 100 });
                context.RealEstates.Add(new RealEstate { Id = 5, Address="Something", Name = "Property5", Price = 101 });
                context.RealEstates.Add(new RealEstate { Id = 6, Address="Anything", Name = "Property6", Price = 102 });
                context.RealEstates.Add(new RealEstate { Id = 7, Address="Whatsoever", Name = "Property7", Price = 103 });
                context.SaveChanges();
                
                var controller = new RealEstatesController(context);

                // Act
                var result = await controller.ReadRealEstate(minPrice: 50, maxPrice: 150, offset: 1, limit: 3, sort: "price") as OkObjectResult;

                var realEstates = result.Value as List<RealEstate>;
                
                // Assert
                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                
                Assert.NotNull(realEstates);
                Assert.True(realEstates.Count == 3);//Assert.Equal(1, realEstates.Count);
                Assert.Equal("Property4", realEstates[0].Name);
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
                var newRealEstate = new RealEstate { Id = 11, Name = "NewProperty", Price = 300 };

                // Act
                var result = controller.CreateRealEstate(newRealEstate) as OkObjectResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);

                var createdRealEstate = result.Value as RealEstate;
                Assert.NotNull(createdRealEstate);
                Assert.Equal(newRealEstate.Id, createdRealEstate.Id);
                Assert.Equal(newRealEstate.Name, createdRealEstate.Name);
                Assert.Equal(newRealEstate.Price, createdRealEstate.Price);
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
                context.RealEstates.Add(new RealEstate { Id = 1, Name = "ExistingProperty", Price = 200 });
                context.SaveChanges();

                var controller = new RealEstatesController(context);
                var existingRealEstate = new RealEstate { Id = 1, Name = "NewProperty", Price = 300 };

                // Act
                var result = controller.CreateRealEstate(existingRealEstate) as BadRequestObjectResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Real Estate Already Exists!", result.Value);
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
                // Adding a real estate with an initial data
                var initialRealEstate = new RealEstate { Id = upId, Name = "Property13", Price = 100, Address = "Address13" };
                context.RealEstates.Add(initialRealEstate);
                context.SaveChanges();

                var controller = new RealEstatesController(context);
                var updatedRealEstate = new RealEstate { Id = upId, Name = "UpdatedProperty", Price = 200, Address = "UpdatedAddress" };

                // Act
                var result = controller.UpdateRealEstate(upId, updatedRealEstate) as OkObjectResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);

                // Verify that the real estate data was updated
                var updatedResult = context.RealEstates.Find(upId);
                Assert.NotNull(updatedResult);
                Assert.Equal(updatedRealEstate.Name, updatedResult.Name);
                Assert.Equal(updatedRealEstate.Price, updatedResult.Price);
                Assert.Equal(updatedRealEstate.Address, updatedResult.Address);
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

                // Assert
                Assert.NotNull(result);
                Assert.Equal(204, result.StatusCode);

                // Verify that the real estate was deleted
                var deletedRealEstate = context.RealEstates.Find(testId);
                Assert.Null(deletedRealEstate);
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