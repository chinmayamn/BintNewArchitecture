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
using Xunit;
using BintTest.Common;
namespace BintTest.Controllers
{
    [ExcludeFromCodeCoverage]
    public class AdminControllerTest:Common.Common
    {
        private readonly ILogger<AdminController> _logger;
        private readonly HttpClient _client = new HttpClient();
        private readonly IHttpContextAccessor _request;
        private readonly IDbFunc _dbf;
        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IDbConstants _dbConstants;
        private readonly IApplicationDbContext _context;
        public AdminControllerTest()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _logger = Substitute.For<ILogger<AdminController>>();
            _environment = Substitute.For<IHostingEnvironment>();
            _dbf = Substitute.For<IDbFunc>();
             var userStore = Substitute.For<IUserStore<ApplicationUser>>();
            _request = Substitute.For<IHttpContextAccessor>();
            _client = Substitute.For<HttpClient>();
            _dbConstants = Substitute.For<IDbConstants>();
            _configuration = Substitute.For<IConfiguration>();
        }
       
        [Fact]
        public void Dashboard_Throws_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();
            AdminController adminController = new AdminController(_request, roleManagerMock, userManagerMock.Object, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var result = adminController.Dashboard();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void MyProfile_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();
            AdminController adminController = new AdminController(_request, roleManagerMock, userManagerMock.Object, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var result = adminController.MyProfile();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void UsdDepositWithdraw_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();
            AdminController adminController = new AdminController(_request, roleManagerMock, userManagerMock.Object, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var result = adminController.UsdDepositWithdraw();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void BgcDepositWithdraw_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();
            AdminController adminController = new AdminController(_request, roleManagerMock, userManagerMock.Object, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var result = adminController.BgcDepositWithdraw();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void UsdPayback_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();
            AdminController adminController = new AdminController(_request, roleManagerMock, userManagerMock.Object, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var result = adminController.UsdPayback();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Members_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();
            AdminController adminController = new AdminController(_request, roleManagerMock, userManagerMock.Object, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var result = adminController.Members();

            //Assert
            Assert.NotNull(result);
        }

       [Fact]
        public void SetKyc_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();
            AdminController adminController = new AdminController(_request, roleManagerMock, userManagerMock.Object, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var recordResult = Record.Exception(() =>
            {
                adminController.SetKyc(Arg.Any<string>(), Arg.Any<string>());
            });
                

            //Assert
            Assert.NotNull(recordResult);
        }

        [Fact]
        public void SiteSettings_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();
            AdminController adminController = new AdminController(_request, roleManagerMock, userManagerMock.Object, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var result = adminController.SiteSettings();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Plans_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();
            AdminController adminController = new AdminController(_request, roleManagerMock, userManagerMock.Object, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var result = adminController.Plans();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void CheckBalance_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();
            AdminController adminController = new AdminController(_request, roleManagerMock, userManagerMock.Object, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var result = adminController.CheckBalance();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ActivityLog_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();
            AdminController adminController = new AdminController(_request, roleManagerMock, userManagerMock.Object, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var result = adminController.ActivityLog();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Usd_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();
            AdminController adminController = new AdminController(_request, roleManagerMock, userManagerMock.Object, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var result = adminController.Usd();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Bgc_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<PartnerController>> logMock = new Mock<ILogger<PartnerController>>();
            AdminController adminController = new AdminController(_request, roleManagerMock, userManagerMock.Object, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var result = adminController.Bgc();

            //Assert
            Assert.NotNull(result);
        }

       
    }
}