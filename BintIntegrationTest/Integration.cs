using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bint;
using Bint.Controllers;
using Bint.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Xunit;
using System.Diagnostics;

namespace BintIntegrationTest
{
    public class IntegrationTestHeroes : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        public IntegrationTestHeroes(CustomWebApplicationFactory<Startup> factory)
        {
  
            _client = factory.CreateClient(); 
        }

        [Fact]
        public async Task Test()
        {
            var stringContent = new StringContent("Partner", Encoding.UTF8, "application/json");
            var httpResponse = await _client.GetAsync("/api/Investor");
             httpResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public void Login()
        {
            var httpResponse =  _client.GetAsync("/account/login").Result;
           
        }

        [Fact]
        public void Register()
        {
            var httpResponse = _client.GetAsync("/account/register").Result;

        }

        [Fact]
        public void GetRoles()
        {
            var httpResponse = _client.GetAsync("/role/index");

        }
    }
}
