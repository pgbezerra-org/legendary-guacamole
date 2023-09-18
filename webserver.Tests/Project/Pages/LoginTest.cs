using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using webserver.Models;
using webserver.Pages.Account;

namespace webserver.Tests.Project.Pages{
    public class LoginModelTests {

        [Fact]
        public async Task SignIn_Successful() {
            
            // Arrange
            var userManagerMock = new Mock<UserManager<BZEAccount>>(
                Mock.Of<IUserStore<BZEAccount>>(),
                null, null, null, null, null, null, null, null);

            var signInManagerMock = new Mock<SignInManager<BZEAccount>>(
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<BZEAccount>>(),
                null, null, null, null); // Add null for the remaining constructor parameters


            var loginModel = new LoginModel(signInManagerMock.Object, userManagerMock.Object);

            var credential = new LoginModel.CredentialInput {
                Email = "employee1234@outlook.com",
                Password = "@Employee1234"
            };

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
            Assert.IsType<RedirectToPageResult>(result); // Check if it redirects to the desired page
            var redirectResult = (RedirectToPageResult)result;
            Assert.Equal("/Privacy", redirectResult.PageName); // Adjust this to your desired redirect page
        }
    }

}