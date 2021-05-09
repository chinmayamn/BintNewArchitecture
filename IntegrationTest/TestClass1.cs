using System;
using System.Collections.Generic;
using System.Text;
using Bint;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTest
{
    public class TestClass1 : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public TestClass1(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public void Test()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response =  client.GetAsync("/role/createrole");
        }
    }
}
