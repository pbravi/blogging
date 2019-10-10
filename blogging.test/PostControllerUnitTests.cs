using blogging.api.Controllers;
using blogging.data;
using blogging.model;
using blogging.service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using blogging.model.auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace blogging.test
{
    [TestClass]
    public class PostControllerUnitTests
    {
        /// <summary>
        /// Test to get all posts
        /// </summary>
        [TestMethod]
        public void GetAll()
        {
            var postController = new PostController(GetMockService());
            var result = postController.Get().Result.Value;

            Assert.IsInstanceOfType(result, typeof(List<Post>));
        }

        /// <summary>
        /// Test to get a post by id
        /// </summary>
        [TestMethod]
        public void GetById()
        {
            var postMock = mockData.FirstOrDefault(p => p.Id == 3);
            var postController = new PostController(GetMockService());
            var result = postController.Get(postMock.Id).Result.Value;

            Assert.IsNotNull(result);
            Assert.AreEqual(postMock.Id, result.Id);
            Assert.AreEqual(postMock.Autor, result.Autor);
            Assert.AreEqual(postMock.Title, result.Title);
            Assert.AreEqual(postMock.Body, result.Body);
        }

        /// <summary>
        /// Test to get a post non existent
        /// </summary>
        [TestMethod]
        public void GetByIdNotFound()
        {
            var postId = 15;
            var postController = new PostController(GetMockService());
            var result = postController.Get(postId).Result;

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        /// <summary>
        /// Test to insert a post
        /// </summary>
        [TestMethod]
        public void Insert()
        {
            var newPost = new Post { Autor = "pbravi", Title = "Test Insert Title", Body = "Test Insert Body", CreationDate = DateTime.Now };
            var postController = new PostController(GetMockService());
            postController.ControllerContext = GetMockContext();
            var result = postController.Post(newPost).Result;
            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
        }

        /// <summary>
        /// Test to delete a another author's post
        /// </summary>
        [TestMethod]
        public void DeleteForbidden()
        {
            var postId = 2;
            var postController = new PostController(GetMockService());
            postController.ControllerContext = GetMockContext();
            var result = postController.Delete(postId).Result;
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(((ObjectResult)result).StatusCode, 403);
        }

        /// <summary>
        /// Test to delete a post from himself
        /// </summary>
        [TestMethod]
        public void DeleteOk()
        {
            var postMock = mockData.First();
            var postController = new PostController(GetMockService());
            var userInfo = new UserInfo { Mail = "pbravi@gmail.com", Password = "F0rT3sT!" };
            postController.ControllerContext = GetMockContext(userInfo);
            var result = postController.Delete(postMock.Id).Result;
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        /// <summary>
        /// Try to delete a non existent post
        /// </summary>
        [TestMethod]
        public void DeleteNotFound()
        {
            var postId = 15;
            var postController = new PostController(GetMockService());
            postController.ControllerContext = GetMockContext();

            var result = postController.Delete(postId).Result;
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #region MockData
        private List<Post> mockData = new List<Post>
        {
            new Post{ Id=1, Autor="pbravi@gmail.com", CreationDate=DateTime.Now, Title="Test title 1", Body="Test body 1" },
            new Post{ Id=2, Autor="pbravi@gmail.com", CreationDate=DateTime.Now, Title="Test title 2", Body="Test body 2" },
            new Post{ Id=3, Autor="pbravi@gmail.com", CreationDate=DateTime.Now, Title="Test title 3", Body="Test body 3" },
            new Post{ Id=4, Autor="pbravi@gmail.com", CreationDate=DateTime.Now, Title="Test title 4", Body="Test body 4" }
        };

        /// <summary>
        /// Get a PostService with a Mock PostRepository
        /// </summary>
        /// <returns></returns>
        private PostService GetMockService()
        {
            var postRepository = new Mock<IRepository<Post>>();
            postRepository.Setup(s => s.GetAllAsync())
                .Returns(
                    Task.FromResult(mockData));
            postRepository.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>((postId) =>
                    Task.FromResult(mockData.FirstOrDefault(p => p.Id == postId)));
            postRepository.Setup(s => s.InsertAsync(It.IsAny<Post>()))
                .Returns<Post>((post) =>
                {
                    post.Id = mockData.Max(m => m.Id) + 1;
                    return Task.Run(() => mockData.Add(post));
                });
            postRepository.Setup(s => s.DeleteAsync(It.IsAny<Post>()))
                .Returns<Post>((post) => Task.Run(() => mockData.Remove(post)));
            return new PostService(postRepository.Object);
        }

        /// <summary>
        /// Get a Mock ControllerContext for operations that require a user logged
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private ControllerContext GetMockContext(UserInfo userInfo = null)
        {
            userInfo = userInfo??new UserInfo { Mail = "for@test.com.ar", Password = "F0rT3sT!" };
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Mail),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.Name, userInfo.Mail),
                        new Claim(ClaimTypes.NameIdentifier, userInfo.Mail),
                    }))
                }
            };            
        }

        #endregion
    }
}
