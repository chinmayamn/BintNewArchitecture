using Bint.Constants;
using Bint.Controllers;
using Bint.Data;
using Bint.Models;
using Bint.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xunit;

namespace BintTest.Controllers
{
    [ExcludeFromCodeCoverage]
    public class InvestorControllerTest:Common.Common
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IDbConstants _dbConstants;
        public InvestorControllerTest()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _dbConstants = Substitute.For<IDbConstants>();
            _configuration = Substitute.For<IConfiguration>();
        }

        [Fact]
        public void Test_Dashboard_Throws_Exception()
        {
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            InvestorController investorController = new InvestorController(MockInvestorLogger.Object, userManagerMock.Object, _context, _configuration, _dbConstants);
            
            //Act
            var result = investorController.Dashboard();

            //Assert
            Assert.NotNull(result);
        }
        [Fact]
        public void Test_Plans_Throws_Exception()
        {
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            InvestorController investorController = new InvestorController(MockInvestorLogger.Object, userManagerMock.Object, _context, _configuration, _dbConstants);
            
            //Act
            var result = investorController.Plans();

            //Assert
            Assert.NotNull(result);
        }
        [Fact]
        public void Test_Myprofile_Throws_Exception()
        {
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            InvestorController investorController = new InvestorController(MockInvestorLogger.Object, userManagerMock.Object, _context, _configuration, _dbConstants);

            //Act
            var result = investorController.MyProfile();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Investments_Throws_Exception()
        {
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            InvestorController investorController = new InvestorController(MockInvestorLogger.Object, userManagerMock.Object, _context, _configuration, _dbConstants);

            //Act
            var result = investorController.Investments();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Partners_Throws_Exception()
        {
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            InvestorController investorController = new InvestorController(MockInvestorLogger.Object, userManagerMock.Object, _context, _configuration, _dbConstants);

            //Act
            var result = investorController.Partners();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_UsdPayback_Throws_Exception()
        {
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            InvestorController investorController = new InvestorController(MockInvestorLogger.Object, userManagerMock.Object, _context, _configuration, _dbConstants);

            //Act
            var result = investorController.UsdPayback();

            //Assert
            Assert.NotNull(result);
        }
        [Fact]
        public void Test_Usd_Throws_Exception()
        {
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            InvestorController investorController = new InvestorController(MockInvestorLogger.Object, userManagerMock.Object, _context, _configuration, _dbConstants);

            //Act
            var result = investorController.Usd();

            //Assert
            Assert.NotNull(result);
        }
        [Fact]
        public void Test_Bgc_Throws_Exception()
        {
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            InvestorController investorController = new InvestorController(MockInvestorLogger.Object, userManagerMock.Object, _context, _configuration, _dbConstants);

            //Act
            var result = investorController.Bgc();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_PartnerDetail_Throws_Exception()
        {
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            InvestorController investorController = new InvestorController(MockInvestorLogger.Object, userManagerMock.Object, _context, _configuration, _dbConstants);

            //Act
            var result = investorController.PartnerDetail();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_DeleteUser_Throws_Exception()
        {
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            InvestorController investorController = new InvestorController(MockInvestorLogger.Object, userManagerMock.Object, _context, _configuration, _dbConstants);

            //Act
            var result = investorController.DeleteUser(Arg.Any<string>());

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_ActivityLog_Throws_Exception()
        {
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            InvestorController investorController = new InvestorController(MockInvestorLogger.Object, userManagerMock.Object, _context, _configuration, _dbConstants);

            //Act
            var result = investorController.ActivityLog();

            //Assert
            Assert.NotNull(result);
        }
    }
}
