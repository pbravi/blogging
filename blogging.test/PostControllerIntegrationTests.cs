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

        /// <summary>
        /// Complete test CRUD operations
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Test to login
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        private async Task Login()
        {            
            //Serialize user info
            StringContent jsonUserInfo = new StringContent(JsonConvert.SerializeObject(userInfo), Encoding.UTF8, "application/json");
            
            //Try to login
            var response = await client.PostAsync(urlLogin, jsonUserInfo);
            if(!response.IsSuccessStatusCode)
                Assert.Fail("Error getting a response for LOGIN");

            //Read result like a UserToken
            var result = JsonConvert.DeserializeObject<UserToken>(
                await response.Content.ReadAsStringAsync());
            //Validations
            Assert.IsInstanceOfType(result, typeof(UserToken));

            //Set token into the client instance
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Tokent);
        }

        /// <summary>
        /// Test to post without being logged
        /// </summary>
        /// <returns></returns>
        private async Task PostUnauthorized()
        {
            foreach (var post in mockData)
            {
                //Serialize post object
                StringContent jsonPost = new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json");
                //Try to post
                var response = await client.PostAsync(url, jsonPost);
                //Validations
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        /// <summary>
        /// Test to post being logged
        /// </summary>
        /// <returns></returns>
        private async Task Post()
        {
            foreach (var post in mockData)
            {
                //Serialize post object
                StringContent jsonPost = new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json");
                //Try to post
                var response = await client.PostAsync(url, jsonPost);

                if (!response.IsSuccessStatusCode)
                    Assert.Fail("Error getting a response for POST");
                //Validations
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            }
        }

        /// <summary>
        /// Test to get all posts
        /// </summary>
        /// <returns></returns>
        private async Task Get()
        {
            //Try to get all posts
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                Assert.Fail("Error getting a response for GET");
            //Deserialize result as List<Post>
            var result = JsonConvert.DeserializeObject<List<Post>>(
                await response.Content.ReadAsStringAsync());
            //Validations
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(mockData.Count, result.Count);
        }

        /// <summary>
        /// Test to get a post by id
        /// </summary>
        /// <param name="postMock"></param>
        /// <returns></returns>
        private async Task GetById(Post postMock)
        {
            //Try to get a post by id
            var response = await client.GetAsync($"{url}/{postMock.Id}");
            if (!response.IsSuccessStatusCode)
                Assert.Fail("Error getting a response for GET BY ID");
            //Deserialize result as a Post
            var result = JsonConvert.DeserializeObject<Post>(
                await response.Content.ReadAsStringAsync());
            //Validations
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(postMock.Id, result.Id);
            Assert.AreEqual(userInfo.Mail, result.Autor);
            Assert.AreEqual(postMock.Title, result.Title);
            Assert.AreEqual(postMock.Body, result.Body);
        }

        /// <summary>
        /// Test to delete a post
        /// </summary>
        /// <param name="postMock"></param>
        /// <returns></returns>
        private async Task Delete(Post postMock)
        {
            //Try to delete a post by id
            var response = await client.DeleteAsync($"{url}/{postMock.Id}");
            if (!response.IsSuccessStatusCode)
                Assert.Fail("Error getting a response for DELETE");

            //Validations
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Test to get a post recently deleted or non existent
        /// </summary>
        /// <param name="postMock"></param>
        /// <returns></returns>
        private async Task GetByIdNotFound(Post postMock)
        {
            //Try to get a post by id
            var response = await client.GetAsync($"{url}/{postMock.Id}");
            //Validations
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

        /// <summary>
        /// Get a Web App Factory with a Web Host Builder to use a In Memory Database
        /// </summary>
        /// <returns></returns>
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
