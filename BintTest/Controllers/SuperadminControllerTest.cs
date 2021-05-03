using Bint.Controllers;
using Bint.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BintTest.Controllers
{
    public class SuperAdminControllerTest
    {
        private static readonly Mock<ILogger<SuperAdminController>> Mock = new Mock<ILogger<SuperAdminController>>();
        private static readonly Mock<IApplicationDbContext> MockAppDbContext = new Mock<IApplicationDbContext>();
        private readonly SuperAdminController _superAdminController = new SuperAdminController(MockAppDbContext.Object, Mock.Object);
        
        [Fact]
        public void Test_Dashboard()
        {

            //mock.Setup(p => p.GetNameById(1)).Returns("Jignesh");
           // string result = home.GetNameById(1);
            //Act
            var result = _superAdminController.Dashboard();

            //Assert
            Assert.NotNull(result);
        }
        [Fact]
        public void Test_Settings()
        {
            //Act
            var result = _superAdminController.Settings();

            //Assert
            Assert.NotNull(result);
        }
        [Fact]
        public void Test_Restricted()
        {
            //Act
            var result = _superAdminController.Restricted();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Statistics()
        {
            //Act
            var result = _superAdminController.Statistics();

            //Assert
            Assert.NotNull(result);
        }
    }
}
