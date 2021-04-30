using Bint.Controllers;
using Bint.Data;
using Bint.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BintTest.Controllers
{
    public class RoleControllerTest
    {
       
        //static Mock<RoleManager<IdentityRole>> _mockRoleManager = new Mock<RoleManager<IdentityRole>>();
        //static Mock<UserManager<ApplicationUser>> _mockUserManager = new Mock<UserManager<ApplicationUser>>();
        //static Mock<ILogger<RoleController>> mock = new Mock<ILogger<RoleController>>();
        //RoleController _roleController = new RoleController(_mockRoleManager.Object,_mockUserManager.Object,mock.Object);
        [Fact]
        public void Test_Index()
        {
            var dbSetMock = new Mock<DbSet<ApplicationUser>>();
            var dbContextMock = new Mock<ApplicationDbContext>();

            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            var logMock = new Mock<ILogger<RoleController>>();

            var roleController = new RoleController(roleManagerMock, userManagerMock.Object, logMock.Object);

            //Act
            var result = roleController.Index() as ViewResult;

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Index_Return_Roles()
        {
            var dbSetMock = new Mock<DbSet<ApplicationUser>>();
            var dbContextMock = new Mock<ApplicationDbContext>();

            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            var logMock = new Mock<ILogger<RoleController>>();

            var roleController = new RoleController(roleManagerMock, userManagerMock.Object, logMock.Object);
           
            //Act
            var result = roleController.Index() as ViewResult;

            //Assert
            Assert.NotNull(result);
        }
        private Mock<UserManager<TIdentityUser>> GetUserManagerMock<TIdentityUser>() where TIdentityUser : IdentityUser
        {
            return new Mock<UserManager<TIdentityUser>>(
                    new Mock<IUserStore<TIdentityUser>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<IPasswordHasher<TIdentityUser>>().Object,
                    new IUserValidator<TIdentityUser>[0],
                    new IPasswordValidator<TIdentityUser>[0],
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<IServiceProvider>().Object,
                    new Mock<ILogger<UserManager<TIdentityUser>>>().Object);
        }

        private Mock<RoleManager<TIdentityRole>> GetRoleManagerMock<TIdentityRole>() where TIdentityRole : IdentityRole
        {
            return new Mock<RoleManager<TIdentityRole>>(
                    new Mock<IRoleStore<TIdentityRole>>().Object,
                    new IRoleValidator<TIdentityRole>[0],
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<ILogger<RoleManager<TIdentityRole>>>().Object);
        }
    }
}
