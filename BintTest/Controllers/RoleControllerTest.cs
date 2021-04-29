using System;
using System.Collections.Generic;
using System.Text;
using Bint.Controllers;
using Bint.Data;
using Bint.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<RoleController>> logMock = new Mock<ILogger<RoleController>>();


            RoleController _roleController = new RoleController(roleManagerMock, userManagerMock.Object, logMock.Object);

            //Act
            var result = _roleController.Index() as ViewResult;

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Index_Return_Roles()
        {
            var dbSetMock = new Mock<DbSet<ApplicationUser>>();
            var dbContextMock = new Mock<ApplicationDbContext>();

            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<RoleController>> logMock = new Mock<ILogger<RoleController>>();


            RoleController _roleController = new RoleController(roleManagerMock, userManagerMock.Object, logMock.Object);
           
            //Act
            var result = _roleController.Index() as ViewResult;

            //Assert
            Assert.NotNull(result);
        }
        Mock<UserManager<TIDentityUser>> GetUserManagerMock<TIDentityUser>() where TIDentityUser : IdentityUser
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

        Mock<RoleManager<TIdentityRole>> GetRoleManagerMock<TIdentityRole>() where TIdentityRole : IdentityRole
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
