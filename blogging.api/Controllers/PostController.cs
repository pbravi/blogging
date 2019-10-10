using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blogging.model;
using blogging.model.auth;
using blogging.service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace blogging.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly BaseService<Post> postService;

        public PostController(BaseService<Post> postService)
        {
            this.postService = postService;
        }
        // GET api/values
        [HttpGet]
        [HttpGet("find/{textSearch?}")]
        public async Task<ActionResult<List<Post>>> Get(string textSearch=null)
        {
            return await postService.GetAllAsync(textSearch);
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "getById")]
        public async Task<ActionResult<Post>> Get(int id)
        {
            var post = await postService.GetByIdAsync(id);
            if (post == null)
                return NotFound();
            return post;
        }

        // POST api/values
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post([FromBody] Post post)
        {
            post.Autor = HttpContext.User.Identity.Name;
            post.CreationDate = DateTime.Now;
            await postService.InsertAsync(post);
            return new CreatedAtRouteResult("getById", new { id = post.Id }, post);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete(int id)
        {
            var post = await postService.GetByIdAsync(id);
            if (post == null)
                return NotFound();
            if (post.Autor != HttpContext.User.Identity.Name)
                return StatusCode(403, "It is forbiden to delete post created by another author"); 
            await postService.DeleteAsync(post);
            return Ok();
        }
    }
}
