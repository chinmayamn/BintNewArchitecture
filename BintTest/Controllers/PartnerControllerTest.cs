using System;
using System.Collections.Generic;
using System.Text;
using Bint.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Bint.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BintTest.Controllers
{
    public class PartnerControllerTest
    {
        [Fact]
        public void Test_Index()
        {
            //var userManagerMock = GetUserManagerMock<ApplicationUser>();
            //userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            //var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            //Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();

            //var _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            //var context = new DefaultHttpContext();
            //_mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //PartnerController _partnerController = new PartnerController(_mockHttpContextAccessor.Object,logMock.Object,roleManagerMock,userManagerMock.Object);
            ////Act
            //var result = _partnerController.Index() as IActionResult;

            ////Assert
            //Assert.NotNull(result);
        }
        //[Fact]
        //public void Test_Dashboard()
        //{
        //    //Act
        //    var result = _partnerController.Dashboard() as IActionResult;

        //    //Assert
        //    Assert.NotNull(result);
        //}
        //[Fact]
        //public void Test_Plans()
        //{
        //    //Act
        //    var result = _partnerController.Plans() as IActionResult;

        //    //Assert
        //    Assert.NotNull(result);
        //}
        //[Fact]
        //public void Test_Myprofile()
        //{
        //    //Act
        //    var result = _partnerController.MyProfile() as IActionResult;

        //    //Assert
        //    Assert.NotNull(result);
        //}
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
