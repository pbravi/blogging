using blogging.api;
using blogging.data;
using blogging.model;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using blogging.model.auth;
using System.Net.Http.Headers;

namespace blogging.test
{
    [TestClass]
    public class PostControllerIntegrationTests
    {
        private WebApplicationFactory<Startup> factory;
        private HttpClient client;
        private const string url = "/api/post";
        private const string urlLogin = "/api/user/login";
        private UserInfo userInfo = new UserInfo { Mail = "for@test.com.ar", Password = "F0rT3sT!" };


        [TestInitialize]
        public void Initialize()
        {
            factory = BuildWithWebHost();
            client = factory.CreateClient();
        }

        [TestMethod]
        public async Task CRUD()
        {
            await PostUnauthorized();
            await Login();
            await Post();
            await Get();
            var postMock = mockData.ElementAt(0);
            await GetById(postMock);
            await Delete(postMock);
            await GetByIdNotFound(postMock);
        }

        [TestMethod]
        private async Task Login()
        {            
            StringContent jsonUserInfo = new StringContent(JsonConvert.SerializeObject(userInfo), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(urlLogin, jsonUserInfo);
            if(!response.IsSuccessStatusCode)
                Assert.Fail("Error getting a response for LOGIN");
            var result = JsonConvert.DeserializeObject<UserToken>(
                await response.Content.ReadAsStringAsync());

            Assert.IsInstanceOfType(result, typeof(UserToken));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Tokent);
        }
        private async Task PostUnauthorized()
        {
            foreach (var post in mockData)
            {
                StringContent jsonPost = new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, jsonPost);
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }
        private async Task Post()
        {
            foreach (var post in mockData)
            {
                StringContent jsonPost = new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, jsonPost);


                if (!response.IsSuccessStatusCode)
                    Assert.Fail("Error getting a response for POST");
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            }
        }

        private async Task Get()
        {
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                Assert.Fail("Error getting a response for GET");
            var result = JsonConvert.DeserializeObject<List<Post>>(
                await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(mockData.Count, result.Count);
        }

        private async Task GetById(Post postMock)
        {

            var response = await client.GetAsync($"{url}/{postMock.Id}");
            if (!response.IsSuccessStatusCode)
                Assert.Fail("Error getting a response for GET BY ID");
            var result = JsonConvert.DeserializeObject<Post>(
                await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(postMock.Id, result.Id);
            Assert.AreEqual(userInfo.Mail, result.Autor);
            Assert.AreEqual(postMock.Title, result.Title);
            Assert.AreEqual(postMock.Body, result.Body);
        }

        private async Task Delete(Post postMock)
        {
            var response = await client.DeleteAsync($"{url}/{postMock.Id}");
            if (!response.IsSuccessStatusCode)
                Assert.Fail("Error getting a response for DELETE");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        private async Task GetByIdNotFound(Post postMock)
        {
            var response = await client.GetAsync($"{url}/{postMock.Id}");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        #region MockData
        private List<Post> mockData = new List<Post>
        {
            new Post{ Id=1, CreationDate=DateTime.Now, Title="Test title 1", Body="Test body 1" },
            new Post{ Id=2, CreationDate=DateTime.Now, Title="Test title 2", Body="Test body 2" },
            new Post{ Id=3, CreationDate=DateTime.Now, Title="Test title 3", Body="Test body 3" },
            new Post{ Id=4, CreationDate=DateTime.Now, Title="Test title 4", Body="Test body 4" }
        };
        private WebApplicationFactory<Startup> BuildWithWebHost()
        {
            factory = new WebApplicationFactory<Startup>();
            return factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    // Create a new service provider.
                    var serviceProvider = new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider();

                    // Add a database context (ApplicationDbContext) using an in-memory 
                    // database for testing.
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                        options.UseInternalServiceProvider(serviceProvider);
                    });

                    // Build the service provider.
                    var sp = services.BuildServiceProvider();

                    // Create a scope to obtain a reference to the database
                    // context (ApplicationDbContext).
                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<AppDbContext>();

                        // Ensure the database is created.
                        db.Database.EnsureCreated();

                    }
                }));
        }


        #endregion
    }
}
