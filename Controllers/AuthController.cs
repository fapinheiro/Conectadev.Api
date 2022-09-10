using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Conectadev.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace Conectadev.Api.Controllers
{
    [Route("v1/auth")]
    public class AuthController : Controller
    {
        private readonly JWTSettings _jwtsettings;

        public AuthController(IOptions<JWTSettings> jwtsettings)
        {
            _jwtsettings = jwtsettings.Value;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserToken>> Authenticate([FromBody] User model) 
        {
            var user = new User 
            {
                Id = 1,
                Name = "Filipe Pinheiro",
                Username = "fapinheiro",
                Email = "fapinheiro@gmail.com",
                Avatar = "/images/avatars/avatar_1.jpeg"
            };

            var token = GenerateAccessToken(user);

            return new UserToken {
                User  = user,
                Token = token
            };
        }

        [HttpGet]
        [Route("users")]
        [Authorize]
        public async Task<ActionResult<User>> GetUserDetails() 
        {
             return new User {
                Id = 1,
                Name = "Filipe Pinheiro",
                Username = "fapinheiro",
                Email = "fapinheiro@gmail.com",
                Avatar = "/images/avatars/avatar_1.jpeg"
            };
        }

        private string GenerateAccessToken(User user) 
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Email.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}