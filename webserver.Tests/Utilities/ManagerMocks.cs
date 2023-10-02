using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using webserver.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;

namespace webserver.Tests.Utilities ;
public static class ManagerMocks {
    public static Mock<UserManager<IdentityUser>> GetUserManagerMock(){
        return new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(),
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<IdentityUser>>(),
            new IUserValidator<IdentityUser>[0],
            new IPasswordValidator<IdentityUser>[0],
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<IdentityUser>>>());
    }

    public static Mock<SignInManager<IdentityUser>> GetSignInManagerMock(Mock<UserManager<IdentityUser>> userManagerMock) {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        var logger = new Mock<ILogger<SignInManager<IdentityUser>>>();
        var schemes = new Mock<IAuthenticationSchemeProvider>();
        var confirmation = new Mock<IUserConfirmation<IdentityUser>>();

        return new Mock<SignInManager<IdentityUser>>(
            userManagerMock.Object,
            contextAccessor.Object,
            userPrincipalFactory.Object,
            options.Object,
            logger.Object,
            schemes.Object,
            confirmation.Object);
    }

    public static Mock<RoleManager<IdentityRole>> GetRoleManagerMock(){
        return new Mock<RoleManager<IdentityRole>>(
            new Mock<IRoleStore<IdentityRole>>().Object,
            new List<IRoleValidator<IdentityRole>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            new Mock<ILogger<RoleManager<IdentityRole>>>()
        );
    }
}