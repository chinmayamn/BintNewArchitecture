using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bint.Constants;
using Bint.Controllers;
using Bint.Data;
using Bint.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.Extensions.Configuration;
using Bint.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BintIntegrationTest
{
    [ExcludeFromCodeCoverage]
    public class PartnerIntegrationTest
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<PartnerController> _logger;
        private readonly IHttpContextAccessor _request;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbFunc _dbf;
        private readonly IDbConstants _dbConstants;
        private readonly IFileHelper _fileHelper;
        private readonly IConfiguration _configuration;
        public PartnerIntegrationTest(IHttpContextAccessor httpContext, ILogger<PartnerController> logger,
            RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
            IApplicationDbContext context, IConfiguration configuration, IDbConstants dbConstants, IFileHelper fileHelper)
        {
            _request = httpContext;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _fileHelper = fileHelper;
            _configuration = configuration;
            _dbConstants = dbConstants;
            _dbf = new DbFunc(logger, configuration, dbConstants);
        }

        [Fact]
        public void Should_Get_Dashboard_Data()
        {
            PartnerController partnerController = new PartnerController(_request, _logger, _roleManager, _userManager, _context,_configuration , _dbConstants,_fileHelper);

            partnerController.Dashboard();

        }
    }
}
