using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using webserver.Pages.Account; // Adjust the namespace as needed
using webserver.Models; // Adjust the namespace as needed

namespace webserver.Tests.Project.Pages.Accounts.Manage {

    public class RegisterTest {
        
        [Fact]
        public async Task CreateAsync_Successful() {

            // Arrange
            var userManagerMock = new Mock<UserManager<BZEmployee>>(
                Mock.Of<IUserStore<BZEmployee>>(),
                null, null, null, null, null, null, null, null);

            var registerModel = new RegisterModel(userManagerMock.Object) {
                RegisterInput = new RegisterModel.RegisterInputModel {
                    Name = "Test User",
                    Email = "test@example.com",
                    Phone = "1234567890",
                    Password = "@Password123",
                    ConfirmPassword = "@Password123"
                }
            };

            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((BZEmployee)null);

            userManagerMock.Setup(m => m.CreateAsync(It.IsAny<BZEmployee>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await userManagerMock.Object.CreateAsync(new BZEmployee(), "Password123");

            // Assert
            Assert.True(result.Succeeded);
        }
    }
}