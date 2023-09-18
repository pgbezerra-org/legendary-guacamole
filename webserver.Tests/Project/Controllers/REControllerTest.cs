using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using webserver.Pages.Account;
using webserver.Models;
using webserver.Data;
using webserver.Controllers;

namespace webserver.Tests.Project.Controllers {
    public class REControllerTest {

        [Fact]
        public async Task ReadRealEstate_ReturnsExpectedResult() {
            // Arrange
            var mockContext = new Mock<WebserverContext>();
            var mockDbSet = new Mock<DbSet<RealEstate>>();
            var realEstates = GetTestRealEstates();  // Método que retorna uma lista de imóveis fictícios
            mockDbSet.As<IQueryable<RealEstate>>().Setup(m => m.Provider).Returns(realEstates.Provider);
            mockDbSet.As<IQueryable<RealEstate>>().Setup(m => m.Expression).Returns(realEstates.Expression);
            mockDbSet.As<IQueryable<RealEstate>>().Setup(m => m.ElementType).Returns(realEstates.ElementType);
            mockDbSet.As<IQueryable<RealEstate>>().Setup(m => m.GetEnumerator()).Returns(realEstates.GetEnumerator());
            mockContext.Setup(c => c.RealEstates).Returns(mockDbSet.Object);

            var controller = new RealEstatesController(mockContext.Object);

            // Act
            //http://localhost:5097/realestates?minPrice=500000&sort=price&offset=1&limit=2
            var result = await controller.ReadRealEstate(minPrice: 50000, maxPrice: 1000000, offset: 1, limit: 2, sort: "price");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<RealEstate>>(okResult.Value);
            Assert.Equal(realEstates.Count(), returnValue.Count());
        }
    }
}