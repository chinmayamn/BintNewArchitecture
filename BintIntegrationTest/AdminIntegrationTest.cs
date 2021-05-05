using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.AccessControl;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Moq;

namespace BintIntegrationTest
{
    public class AdminIntegrationTest:Common
    {
        private readonly IConfiguration _configuration;
        private readonly IApplicationDbContext _context;
        private readonly IDbFunc _dbf;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;
        private readonly IMessage _message;
        private readonly IHttpContextAccessor _request;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbConstants _dbConstants;
        private readonly IFileHelper _fileHelper;
        public AdminIntegrationTest(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContext,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IConfiguration configuration,
            IApplicationDbContext context,
            IMessage message, IDbConstants dbConstants, IFileHelper fileHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _request = httpContext;
            _emailSender = emailSender;
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _message = message;
            _dbConstants = dbConstants;
            _fileHelper = fileHelper;
            _dbf = new DbFunc(_logger, configuration, dbConstants);
        }

        
    }
}
