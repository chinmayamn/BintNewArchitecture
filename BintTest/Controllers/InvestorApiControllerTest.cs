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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xunit;

namespace BintTest.Controllers
{
    public class InvestorApiControllerTest:Common.Common
    {
        private readonly IFileHelper _fileHelper;
        public InvestorApiControllerTest()
        {
            _fileHelper = Substitute.For<IFileHelper>();
        }

        [Fact]
        public void TetherUpdate_Throws_Exception()
        {
            var userManagerMock = GetUserManagerMock<ApplicationUser>();
            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new ApplicationUser()));
            InvestorApiController investorController = new InvestorApiController(userManagerMock.Object, MockInvestorApiLogger.Object,_fileHelper);

            //Act
            var result = investorController.TetherUpdate(Arg.Any<string>(),Arg.Any<IFormFile>());

            //Assert
            Assert.NotNull(result);
        }
    }
}
