using Bint.Controllers;
using Bint.Data;
using Bint.Models;
using Bint.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using Moq;

namespace BintTest.Controllers
{
    public class AdminControllerTest
    {
        private static readonly TimeZoneInfo IndianZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private readonly HttpClient _client = new HttpClient();
        private readonly IHttpContextAccessor _request;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbFunc _dbFunc;
        private readonly IHostingEnvironment _environment;
        private static readonly Mock<ApplicationDbContext> MockAppDbContext = new Mock<ApplicationDbContext>();
        public AdminControllerTest(ApplicationDbContext dbContext)
        {
            _dbContextOptions = Substitute.For<DbContextOptions<ApplicationDbContext>>();
            _context = Substitute.For<ApplicationDbContext>(Arg.Any<DbContextOptions<ApplicationDbContext>>());
            _request = Substitute.For<IHttpContextAccessor>();
            _roleManager = Substitute.For<RoleManager<IdentityRole>>();
            _userManager = Substitute.For<UserManager<ApplicationUser>>();
            _environment = Substitute.For<IHostingEnvironment>();
            _logger = Substitute.For<ILogger<AdminController>>();
            _dbFunc = Substitute.For<IDbFunc>();
        }

        [Fact]
        public void Should_AdminDashboard_Load_Successfully()
        {
       
        // ApplicationDbContext applicationDbContext = new ApplicationDbContext(_dbContextOptions);
        AdminController adminController = new AdminController(_request, _roleManager, _userManager, _environment, _logger, _context, _dbFunc);
            adminController.Dashboard();
        }
    }
}