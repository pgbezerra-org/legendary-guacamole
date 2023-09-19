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
    }
}