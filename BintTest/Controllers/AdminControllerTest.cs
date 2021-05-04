using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using Bint.Constants;
using Bint.Controllers;
using Bint.Data;
using Bint.Models;
using Bint.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace BintTest.Controllers
{
    [ExcludeFromCodeCoverage]
    public class AdminControllerTest
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private readonly HttpClient _client = new HttpClient();
        private readonly IHttpContextAccessor _request;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbFunc _dbf;
        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IDbConstants _dbConstants;
        private readonly DbContextOptions _dbContextOptions;
        public AdminControllerTest(ApplicationDbContext dbContext)
        {
            _context = dbContext;
            _logger = Substitute.For<ILogger<AdminController>>();
            _environment = Substitute.For<IHostingEnvironment>();
            _dbf = Substitute.For<IDbFunc>();
            _userManager = Substitute.For<UserManager<ApplicationUser>>();
            _roleManager = Substitute.For<RoleManager<IdentityRole>>();
            _request = Substitute.For<IHttpContextAccessor>();
            _client = Substitute.For<HttpClient>();
            _dbConstants = Substitute.For<IDbConstants>();
            _configuration = Substitute.For<IConfiguration>();
        }

        [Fact]
        public void Dashboard_Throws_Exception()
        {
            //Arrange
            AdminController adminController = new AdminController(_request, _roleManager, _userManager, _environment, _logger, _context, _configuration, _dbConstants);

            //Act
            var result = adminController.Dashboard().Throws(new Exception());

            //Assert
            Assert.NotNull(result);
        }
    }
}