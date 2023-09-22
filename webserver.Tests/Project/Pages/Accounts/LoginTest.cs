using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using webserver.Models;
using webserver.Pages.Account;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;

namespace webserver.Tests.Project.Pages{
    public class LoginModelTests {

        public Mock<UserManager<BZEAccount>> GetUserManagerMock(){
            return new Mock<UserManager<BZEAccount>>(
                Mock.Of<IUserStore<BZEAccount>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<BZEAccount>>(),
                new IUserValidator<BZEAccount>[0],
                new IPasswordValidator<BZEAccount>[0],
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<BZEAccount>>>());
        }

        public Mock<SignInManager<BZEAccount>> GetSignInManagerMock(Mock<UserManager<BZEAccount>> userManagerMock) {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<BZEAccount>>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<BZEAccount>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<BZEAccount>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<BZEAccount>>();

            return new Mock<SignInManager<BZEAccount>>(
                userManagerMock.Object,
                contextAccessor.Object,
                userPrincipalFactory.Object,
                options.Object,
                logger.Object,
                schemes.Object,
                confirmation.Object);
        }


        [Fact]
        public async Task SignIn_Successful() {

            // Arrange
            var userManagerMock =  GetUserManagerMock();

            var signInManagerMock = GetSignInManagerMock(userManagerMock);

            var loginModel = new LoginModel(signInManagerMock.Object, userManagerMock.Object);

            var credential = new LoginModel.CredentialInput {
                Email = "employee1234@outlook.com",
                Password = "@Employee1234"
            };

            loginModel.Credential = credential;

            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new BZEAccount
                {
                    Id = "5c6cbccc-b325-415d-9cb6-9287d582ee46", // Replace with a valid user ID
                    UserName = "employee1234", // Replace with a valid username
                    Email = "employee1234@outlook.com" // Replace with a valid email
                });

            signInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<BZEAccount>(), It.IsAny<string>(), true, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success); // Specify the Identity SignInResult

            // Act
            var result = await loginModel.OnPostAsync();

            // Assert
            var signInResult = await signInManagerMock.Object.PasswordSignInAsync(It.IsAny<BZEAccount>(), It.IsAny<string>(), true, false);
            Assert.True(signInResult.Succeeded);
        }

        [Theory]
        [InlineData("invalid@outlook.com", "InvalidPassword1")]
        [InlineData("failtest@hotmail.com", "FailTestPassword2")]
        [InlineData("success@gmail.com", "WrongPassword3")]
        public async Task SignIn_Failure(string email, string password) {
            // Arrange
            var userManagerMock = GetUserManagerMock();

            var signInManagerMock = GetSignInManagerMock(userManagerMock);

            var loginModel = new LoginModel(signInManagerMock.Object, userManagerMock.Object);

            var credential = new LoginModel.CredentialInput {
                Email = email,
                Password = password
            };

            loginModel.Credential = credential;

            userManagerMock.Setup(m => m.FindByEmailAsync(email))
                .ReturnsAsync(new BZEAccount {
                    Id = "5c6cbccc-b325-415d-9cb6-9287d582ee46", // Replace with a valid user ID
                    UserName = "employee1234", // Replace with a valid username
                    Email = "employee1234@outlook.com" // Replace with a valid email
                });

            signInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<BZEAccount>(), It.IsAny<string>(), true, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed); // Specify a failed SignInResult

            // Act
            var result = await loginModel.OnPostAsync();

            // Assert
            var signInResult = await signInManagerMock.Object.PasswordSignInAsync(It.IsAny<BZEAccount>(), It.IsAny<string>(), true, false);
            Assert.False(signInResult.Succeeded);
        }
    }
}