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
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BintTest.Controllers
{
    [ExcludeFromCodeCoverage]
    public class AdminApiControllerTest : Common.Common
    {
        private readonly ILogger<AdminApiController> _logger;
        private readonly HttpClient _client = new HttpClient();
        private readonly IHttpContextAccessor _request;
        private readonly IDbFunc _dbf;
        private readonly IConfiguration _configuration;
        private readonly IDbConstants _dbConstants;
        private readonly IApplicationDbContext _context;
        private readonly ILogger<Message> _messageLogger;
        private readonly IFileHelper _fileHelper;
        private readonly IMessage _message;
        
        public AdminApiControllerTest()
        {
            _context = Substitute.For<IApplicationDbContext>();
            
            _dbf = Substitute.For<IDbFunc>();
            _request = Substitute.For<IHttpContextAccessor>();
            _client = Substitute.For<HttpClient>();
            _dbConstants = Substitute.For<IDbConstants>();
            _configuration = Substitute.For<IConfiguration>();
            _messageLogger = Substitute.For<ILogger<Message>>();
            _fileHelper = Substitute.For<IFileHelper>();
            _message = Substitute.For<IMessage>();
            
        }

        [Fact]
        public void UserRoles_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, _logger, _context, _messageLogger,_message,_fileHelper, _dbConstants);

            //Act
            var result = adminApiController.UserRoles();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void CreateRole_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, _logger, _context, _messageLogger, _message, _fileHelper, _dbConstants);

            //Act
            var result = adminApiController.CreateRole(Arg.Any<string>());

            //Assert
            Assert.NotNull(result);
        }


        [Fact]
        public void ProfilePic_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
       
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, LogMock.Object, _context, _messageLogger, _message, _fileHelper, _dbConstants);

            //Act
            var result = adminApiController.ProfilePic(Arg.Any<IFormFile>());

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void UploadDocs_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, _logger, _context, _messageLogger, _message, _fileHelper, _dbConstants);

            //Act
            var result = adminApiController.UploadDocs(Arg.Any<IFormFile>(),Arg.Any<string>());

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void TetherUpdate_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, _logger, _context, _messageLogger, _message, _fileHelper, _dbConstants);

            //Act
            var result = adminApiController.TetherUpdate(Arg.Any<string>(),Arg.Any<IFormFile>(), Arg.Any<string>());

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfirmDeposit_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, _logger, _context, _messageLogger, _message, _fileHelper, _dbConstants);

            //Act
            var result = adminApiController.ConfirmDeposit(Arg.Any<int>(),Arg.Any<string>());

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfirmWithdraw_Throw_Exception()
        {
            //Arrange
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, _logger, _context, _messageLogger, _message, _fileHelper, _dbConstants);

            //Act
            var result = adminApiController.ConfirmWithdraw(Arg.Any<int>(), Arg.Any<string>(),Arg.Any<string>());

            //Assert
            Assert.NotNull(result);
        }

    }
}
