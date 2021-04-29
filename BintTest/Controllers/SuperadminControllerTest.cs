using System;
using System.Collections.Generic;
using System.Text;
using Bint.Controllers;
using Bint.Data;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;

namespace BintTest.Controllers
{
    public class SuperadminControllerTest
    {
        static Mock<ILogger<SuperAdminController>> mock = new Mock<ILogger<SuperAdminController>>();
        static Mock<IApplicationDBContext> mockAppDBContext = new Mock<IApplicationDBContext>();
        SuperAdminController _superAdminController = new SuperAdminController(mockAppDBContext.Object,mock.Object);
        public SuperadminControllerTest()
        {
            

           
        }

        
        [Fact]
        public void Test_Dashboard()
        {

            //mock.Setup(p => p.GetNameById(1)).Returns("Jignesh");
           // string result = home.GetNameById(1);
            //Act
            var result = _superAdminController.Dashboard() as IActionResult;

            //Assert
            Assert.NotNull(result);
        }
        [Fact]
        public void Test_Settings()
        {
            //Act
            var result = _superAdminController.Settings() as IActionResult;

            //Assert
            Assert.NotNull(result);
        }
        [Fact]
        public void Test_Restricted()
        {
            //Act
            var result = _superAdminController.Restricted() as IActionResult;

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Statistics()
        {
            //Act
            var result = _superAdminController.Statistics() as IActionResult;

            //Assert
            Assert.NotNull(result);
        }
    }
}
