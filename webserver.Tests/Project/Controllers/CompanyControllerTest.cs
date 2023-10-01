using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using webserver.Controllers;
using webserver.Models;
using webserver.Data;
using webserver.Models.DTOs;

using Microsoft.Data.Sqlite;

namespace webserver.Tests.Project.Controllers {
    public class CompanyControllerTest : IDisposable {
        private readonly DbContextOptions<WebserverContext> _options;
        private readonly WebserverContext _context;

        private readonly CompanyController _controller;

        public CompanyControllerTest() {
            var connectionStringBuilder = new SqliteConnectionStringBuilder {
                DataSource = ":memory:"
            };
            var connection = new SqliteConnection(connectionStringBuilder.ToString());

            _options = new DbContextOptionsBuilder<WebserverContext>()
                .UseSqlite(connection)
                .Options;

            _context = new WebserverContext(_options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();
        }

        public void Dispose() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void ReadCompany_ReturnsOk_WhenUserExists() {
            // Arrange
            var companyId = "existingUserId";
            var company = new Company { Id = companyId, /* Other properties */ };
            _context.Company.Add(company);
            _context.SaveChanges();

            // Act
            var result = _controller.ReadCompany(companyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var companyDTO = Assert.IsType<CompanyDTO>(okResult.Value);
            Assert.Equal(companyId, companyDTO.Id);
        }

        [Fact]
        public void ReadCompany_ReturnsNotFound_WhenUserDoesNotExist() {
            // Arrange
            var nonExistentCompanyId = "nonExistentUserId";

            // Act
            var result = controller.ReadCompany(nonExistentCompanyId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}