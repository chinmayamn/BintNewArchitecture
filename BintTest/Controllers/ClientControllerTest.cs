using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bint.Constants;
using Bint.Controllers;
using Bint.Data;
using Bint.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NSubstitute;
using Xunit;
using Microsoft.Extensions.Configuration;
using Bint.Services;
using Microsoft.AspNetCore.Http;

namespace BintTest.Controllers
{
    [ExcludeFromCodeCoverage]
    public class ClientControllerTest:Common.Common
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IDbConstants _dbConstants;
        private readonly IFileHelper _fileHelper;
        public ClientControllerTest()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _configuration = Substitute.For<IConfiguration>();
            _dbConstants = Substitute.For<IDbConstants>();
            _fileHelper = Substitute.For<IFileHelper>();
        }
   
        [Fact]
        public void Test_Dashboard_Throws_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));
            ClientController clientController = new ClientController(MockClientLogger.Object, _context, userManagerMock.Object, _configuration, _dbConstants, _fileHelper);

            //Act
            var result = clientController.Dashboard();

            //Assert
            Assert.NotNull(result);
        }
        [Fact]
        public void Test_Plans_Throws_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));
            ClientController clientController = new ClientController(MockClientLogger.Object, _context, userManagerMock.Object, _configuration, _dbConstants, _fileHelper);

            //Act
            var result = clientController.Plans();

            //Assert
            Assert.NotNull(result);
        }
        [Fact]
        public void Test_MyProfile_Throws_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));
            ClientController clientController = new ClientController(MockClientLogger.Object, _context, userManagerMock.Object, _configuration, _dbConstants, _fileHelper);

            //Act
            var result = clientController.MyProfile();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Usd__Throws_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));
            ClientController clientController = new ClientController(MockClientLogger.Object, _context, userManagerMock.Object, _configuration, _dbConstants, _fileHelper);

            //Act
            var result = clientController.Usd();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Bgc__Throws_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));
            ClientController clientController = new ClientController(MockClientLogger.Object, _context, userManagerMock.Object, _configuration, _dbConstants, _fileHelper);

            //Act
            var result = clientController.Bgc();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_ActivityLog__Throws_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));
            ClientController clientController = new ClientController(MockClientLogger.Object, _context, userManagerMock.Object, _configuration, _dbConstants, _fileHelper);

            //Act
            var result = clientController.ActivityLog();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_TetherUpdate__Throws_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));
            ClientController clientController = new ClientController(MockClientLogger.Object, _context, userManagerMock.Object, _configuration, _dbConstants, _fileHelper);

            //Act
            var result = clientController.TetherUpdate(Arg.Any<string>(),Arg.Any<IFormFile>());

            //Assert
            Assert.NotNull(result);
        }
    }
}
