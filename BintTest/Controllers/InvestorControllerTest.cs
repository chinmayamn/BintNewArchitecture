using System;
using System.Collections.Generic;
using System.Text;
using Bint.Controllers;
using Bint.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BintTest.Controllers
{
    public class InvestorControllerTest
    {
        static Mock<ILogger<InvestorController>> mock = new Mock<ILogger<InvestorController>>();

         

        //[Fact]
        //public void Test_Index()
        //{
        //    var userManagerMock = GetUserManagerMock<ApplicationUser>();
        //    userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

        //    InvestorController _investorController = new InvestorController(mock.Object, userManagerMock.Object);
        //    //Act
        //    var result = _investorController.Index() as IActionResult;

        //    //Assert
        //    Assert.NotNull(result);
        //}
        //[Fact]
        //public void Test_Dashboard()
        //{
        //    //Act
        //    var result = _investorController.Dashboard() as IActionResult;

        //    //Assert
        //    Assert.NotNull(result);
        //}
        //[Fact]
        //public void Test_Plans()
        //{
        //    //Act
        //    var result = _investorController.Plans() as IActionResult;

        //    //Assert
        //    Assert.NotNull(result);
        //}
        //[Fact]
        //public void Test_Myprofile()
        //{
        //    //Act
        //    var result = _investorController.MyProfile() as IActionResult;

        //    //Assert
        //    Assert.NotNull(result);
        //}

        //[Fact]
        //public void Test_Investments()
        //{
        //    //Act
        //    var result = _investorController.Investments() as IActionResult;

        //    //Assert
        //    Assert.NotNull(result);
        //}

        //[Fact]
        //public void Test_Partners()
        //{
        //    //Act
        //    var result = _investorController.Partners() as IActionResult;

        //    //Assert
        //    Assert.NotNull(result);
        //}

        //[Fact]
        //public void Test_Bitcoin()
        //{
        //    //Act
        //    var result = _investorController.Bitcoin() as IActionResult;

        //    //Assert
        //    Assert.NotNull(result);
        //}
        //[Fact]
        //public void Test_PartnerDetail()
        //{
        //    //Act
        //    var result = _investorController.PartnerDetail() as IActionResult;

        //    //Assert
        //    Assert.NotNull(result);
        //}
        //[Fact]
        //public void Test_Register()
        //{
        //    //Act
        //    var result = _investorController.Register() as IActionResult;

        //    //Assert
        //    Assert.NotNull(result);
        //}
        //Mock<UserManager<TIDentityUser>> GetUserManagerMock<TIDentityUser>() where TIDentityUser : IdentityUser
        //{
        //    return new Mock<UserManager<TIDentityUser>>(
        //        new Mock<IUserStore<TIDentityUser>>().Object,
        //        new Mock<IOptions<IdentityOptions>>().Object,
        //        new Mock<IPasswordHasher<TIDentityUser>>().Object,
        //        new IUserValidator<TIDentityUser>[0],
        //        new IPasswordValidator<TIDentityUser>[0],
        //        new Mock<ILookupNormalizer>().Object,
        //        new Mock<IdentityErrorDescriber>().Object,
        //        new Mock<IServiceProvider>().Object,
        //        new Mock<ILogger<UserManager<TIDentityUser>>>().Object);
        //}

        //Mock<RoleManager<TIdentityRole>> GetRoleManagerMock<TIdentityRole>() where TIdentityRole : IdentityRole
        //{
        //    return new Mock<RoleManager<TIdentityRole>>(
        //        new Mock<IRoleStore<TIdentityRole>>().Object,
        //        new IRoleValidator<TIdentityRole>[0],
        //        new Mock<ILookupNormalizer>().Object,
        //        new Mock<IdentityErrorDescriber>().Object,
        //        new Mock<ILogger<RoleManager<TIdentityRole>>>().Object);
        //}
    }
}
