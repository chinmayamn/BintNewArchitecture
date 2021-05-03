using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using Bint.Controllers;
using Bint.Data;
using Bint.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace BintTest.Controllers
{
    [ExcludeFromCodeCoverage]
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
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            //var _mockEnvironment = new Mock<IHostingEnvironment>();
            //_mockEnvironment.Setup(m => m.EnvironmentName)
            //  .Returns("Hosting:UnitTestEnvironment");
            var mockHostingEnvironment = new Mock<IHostingEnvironment>(MockBehavior.Strict);

            var mockLogger = new Mock<ILogger<AdminController>>();

            var dbSetMock = new Mock<DbSet<ApplicationUser>>();
            var dbContextMock = new Mock<ApplicationDbContext>();
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new ApplicationUser()));

            var roleManagerMock = GetRoleManagerMock<IdentityRole>().Object;
            var logMock = new Mock<ILogger<RoleController>>();

            var adb = new AdminDashboard();


            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            //   var mockClient = new Mock<HttpClient>();
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/")
            };

            //mockClient.Object.BaseAddress = new Uri("http://localhost:123");

            // Arrange
            //  AdminController adm = new AdminController(_mockHttpContextAccessor.Object, roleManagerMock, userManagerMock.Object, mockHostingEnvironment.Object, _mockLogger.Object);

            // Act
            // var result = adm.Dashboard() as IActionResult;

            // Assert
            //  Assert.NotNull(result);
        }

        private Mock<UserManager<TIDentityUser>> GetUserManagerMock<TIDentityUser>() where TIDentityUser : IdentityUser
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