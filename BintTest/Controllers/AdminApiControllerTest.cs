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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bint.Data.Migrations;
using BintTest.Common;
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
        private readonly IEmailSender _emailSender;

        public AdminApiControllerTest()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _emailSender = Substitute.For<IEmailSender>();
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
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, MockAdminApiLogger.Object, _context, _messageLogger,_message,_fileHelper, _dbConstants);

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
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, MockAdminApiLogger.Object, _context, _messageLogger, _message, _fileHelper, _dbConstants);

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
       
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, MockAdminApiLogger.Object, _context, _messageLogger, _message, _fileHelper, _dbConstants);

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
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, MockAdminApiLogger.Object, _context, _messageLogger, _message, _fileHelper, _dbConstants);

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
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, MockAdminApiLogger.Object, _context, _messageLogger, _message, _fileHelper, _dbConstants);

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
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, MockAdminApiLogger.Object, _context, _messageLogger, _message, _fileHelper, _dbConstants);

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
            AdminApiController adminApiController = new AdminApiController(roleManagerMock, userManagerMock.Object, MockAdminApiLogger.Object, _context, _messageLogger, _message, _fileHelper, _dbConstants);

            //Act
            var result = adminApiController.ConfirmWithdraw(Arg.Any<int>(), Arg.Any<string>(),Arg.Any<string>());

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void CreateRole_CreateUsers()
        {
            //Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "Test",
                    Id = Guid.NewGuid().ToString(),
                    Email = "test@test.it"
                }

            }.AsQueryable();

            var fakeUserManager = new Mock<FakeUserManager>();

            fakeUserManager.Setup(x => x.Users)
                .Returns(users);

            fakeUserManager.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            fakeUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            fakeUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            var signInManager = new Mock<FakeSignInManager>();

            signInManager.Setup(
                    x => x.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;

            roleManagerMock.CreateAsync(new IdentityRole("Partner"));
            roleManagerMock.CreateAsync(new IdentityRole("Client"));
            roleManagerMock.CreateAsync(new IdentityRole("Investor"));
            AccountController adminController = new AccountController(fakeUserManager.Object, signInManager.Object, roleManagerMock, _request, _emailSender, MockAccountLogger.Object, _configuration, _context, _message, _dbConstants, _fileHelper);
            adminController.Register(); 
            //_roleManager.CreateAsync(new IdentityRole("Admin"));
            //var firstUser = new ApplicationUser()
            //{
            //    UserName = "admin@admin.com",
            //    Email = "admin@admin.com",
            //    PasswordHash = ""
            //};
            //_userManager.CreateAsync(firstUser, "Mangala@123");
            //_userManager.AddToRoleAsync(firstUser, "Admin");
        }
    }
}
