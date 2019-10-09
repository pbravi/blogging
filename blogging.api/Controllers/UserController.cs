using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using blogging.model.auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace blogging.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        //[HttpPost("Create")]
        //public async Task<ActionResult<UserToken>> Create([FromBody]UserInfo userInfo)
        //{
        //    var user = new ApplicationUser { UserName = userInfo.Mail, Email = userInfo.Mail };
        //    var result = await userManager.CreateAsync(user, userInfo.Password);
        //    if (result.Succeeded)
        //        return BuildToken(userInfo);
        //    else
        //        return BadRequest();
        //}
        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login([FromBody]UserInfo userInfo)
        {            
            //Attemp to login
            var result = await signInManager.PasswordSignInAsync(userInfo.Mail, userInfo.Password, true, false);
            if (result.Succeeded)
                return BuildToken(userInfo);
            else
            {
                //If user doesn't exist then create and signin
                var user = new ApplicationUser { UserName = userInfo.Mail, Email = userInfo.Mail };
                var resultCreate = await userManager.CreateAsync(user, userInfo.Password);
                if (resultCreate.Succeeded)
                {
                    await signInManager.PasswordSignInAsync(user, userInfo.Password, true, false);
                    return BuildToken(userInfo);
                }
                else
                    return BadRequest(resultCreate.Errors);
            }
        }

        private UserToken BuildToken(UserInfo userInfo)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Mail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, userInfo.Mail),
                new Claim(ClaimTypes.NameIdentifier, userInfo.Mail),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken
            (
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new UserToken { Tokent = new JwtSecurityTokenHandler().WriteToken(token), Expiration = expiration };
        }
    }
}