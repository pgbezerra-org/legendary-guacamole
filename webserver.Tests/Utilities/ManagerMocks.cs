using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;
using webserver.Pages.Account;
using webserver.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;

namespace webserver.Tests.Utilities ;
public class ManagerMocks {
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

    public Mock<RoleManager<IdentityRole>> GetRoleManagerMock(){
        return new Mock<RoleManager<IdentityRole>>(
            new Mock<IRoleStore<IdentityRole>>().Object,
            new List<IRoleValidator<IdentityRole>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null);
    }
}