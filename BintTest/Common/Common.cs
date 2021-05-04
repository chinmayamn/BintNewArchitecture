using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using Bint.Controllers;

namespace BintTest.Common
{
    [ExcludeFromCodeCoverage]
    public class Common
    {
        public Mock<UserManager<TIDentityUser>> GetUserManagerMock<TIDentityUser>() where TIDentityUser : IdentityUser
        {
            return new Mock<UserManager<TIDentityUser>>(
                new Mock<IUserStore<TIDentityUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<TIDentityUser>>().Object,
                new IUserValidator<TIDentityUser>[0],
                new IPasswordValidator<TIDentityUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<TIDentityUser>>>().Object);
        }

        public Mock<RoleManager<TIdentityRole>> GetRoleManagerMock<TIdentityRole>() where TIdentityRole : IdentityRole
        {
            return new Mock<RoleManager<TIdentityRole>>(
                new Mock<IRoleStore<TIdentityRole>>().Object,
                new IRoleValidator<TIdentityRole>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<TIdentityRole>>>().Object);
        }

        public Mock<ILogger<AdminApiController>> MockAdminApiLogger = new Mock<ILogger<AdminApiController>>();
        public Mock<ILogger<AdminController>> MockAdminLogger = new Mock<ILogger<AdminController>>();
        public Mock<ILogger<ClientController>> MockClientLogger = new Mock<ILogger<ClientController>>();
        public Mock<ILogger<ClientApiController>> MockClientApiLogger = new Mock<ILogger<ClientApiController>>();
        public Mock<ILogger<InvestorController>> MockInvestorLogger = new Mock<ILogger<InvestorController>>();
        public Mock<ILogger<InvestorApiController>> MockInvestorApiLogger = new Mock<ILogger<InvestorApiController>>();
        public Mock<ILogger<PartnerController>> MockPartnerLogger = new Mock<ILogger<PartnerController>>();
       
    }
}
