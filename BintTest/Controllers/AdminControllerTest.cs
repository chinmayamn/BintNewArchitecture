using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Bint.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Bint.Controllers;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Bint.Data;

namespace BintTest.Controllers
{
    public class AdminControllerTest
    {
        
        //private IHttpContextAccessor _request;
        //HttpClient client = new HttpClient();
        //private RoleManager<IdentityRole> _roleManager;
        //private UserManager<ApplicationUser> _userManager;
        //private IHostingEnvironment Environment;
        //private readonly ILogger<AdminController> _logger;

        //public AdminControllerTest(IHttpContextAccessor httpcontext, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> usermanager, IHostingEnvironment _environment, ILogger<AdminController> logger)
        //{
        //    _request = httpcontext;
        //    string Baseurl = $"{_request.HttpContext.Request.Scheme}://{_request.HttpContext.Request.Host}";
        //    client.BaseAddress = new Uri(Baseurl);
        //    _roleManager = roleManager; _userManager = usermanager;
        //    Environment = _environment;
        //    _logger = logger;
        //}

        [Fact]
        public void AdminDashboard_Model_NotEmpty()
        {
            var _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //var _mockEnvironment = new Mock<IHostingEnvironment>();
            //_mockEnvironment.Setup(m => m.EnvironmentName)
            //  .Returns("Hosting:UnitTestEnvironment");
            var mockHostingEnvironment = new Mock<IHostingEnvironment>(MockBehavior.Strict);

            var _mockLogger = new Mock<ILogger<AdminController>>();
      
            var dbSetMock = new Mock<DbSet<ApplicationUser>>();
            var dbContextMock = new Mock<ApplicationDbContext>();
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<String>())).Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            Mock<ILogger<RoleController>> logMock = new Mock<ILogger<RoleController>>();
         
            AdminDashboard adb = new AdminDashboard();


            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
         //   var mockClient = new Mock<HttpClient>();
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            //mockClient.Object.BaseAddress = new Uri("http://localhost:123");

            // Arrange
          //  AdminController adm = new AdminController(_mockHttpContextAccessor.Object, roleManagerMock, userManagerMock.Object, mockHostingEnvironment.Object, _mockLogger.Object);

            // Act
           // var result = adm.Dashboard() as IActionResult;

            // Assert
          //  Assert.NotNull(result);
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
