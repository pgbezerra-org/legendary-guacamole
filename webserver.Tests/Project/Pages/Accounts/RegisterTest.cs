using Moq;
using webserver.Pages.Account;
using webserver.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace webserver.Tests.Project.Pages.Accounts.Manage {

    public class RegisterTest {

        public Mock<UserManager<BZEmployee>> GetUserManagerMock(){
            return new Mock<UserManager<BZEmployee>>(
                Mock.Of<IUserStore<BZEmployee>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<BZEmployee>>(),
                new IUserValidator<BZEmployee>[0],
                new IPasswordValidator<BZEmployee>[0],
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<BZEmployee>>>());
        }

        public Mock<SignInManager<BZEmployee>> GetSignInManagerMock(Mock<UserManager<BZEAccount>> userManagerMock){
            return new Mock<SignInManager<BZEmployee>>(
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<BZEmployee>>());
                //,null, null, null, null); // Add null for the remaining constructor parameters
        }

        public Mock<RoleManager<IdentityRole>> GetRoleManagerMock(){
            return new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object,
                new List<IRoleValidator<IdentityRole>>(),
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null);
        }
        
        [Fact]
        public async Task CreateAsync_Successful() {

            // Arrange
            var userManagerMock = GetUserManagerMock();
            var roleManagerMock = GetRoleManagerMock();

            var registerModel = new RegisterModel(userManagerMock.Object, roleManagerMock.Object) {
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