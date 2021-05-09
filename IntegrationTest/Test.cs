using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Bint;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTest
{
    public class Test : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        public Test(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            }); ;
        }

        [Fact]
        public void Firsttest()
        {

            var defaultPage =  _client.GetAsync("/role/createrole");
       

        }

    }
}
