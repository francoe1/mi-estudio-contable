using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiEstudio.Server.Data.Contexts;
using MiEstudio.Server.Data.Models;
using MiEstudio.Server.Data.Resources;
using MiEstudio.Shared.Data.Resources;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MiEstudio.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(DataContext dataContext, IConfiguration configuration)
        {
            _context = dataContext;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginInput input)
        {
            UserModel user = await _context.Users.Where(x => x.User.ToLower().Equals(input.UserName.ToLower())).FirstOrDefaultAsync();
            if (user == null) return BadRequest("User not exist");
            if (!user.Password.Equals(input.Password, StringComparison.CurrentCulture)) return BadRequest("Wrong credentials");
            return Ok(CreateToken(user.ToResource()));
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] UserStoreInput input)
        {
            if (UserType < input.Type) return Forbid($"Require a permission less to {UserType}");

            UserModel user = new UserModel
            {
                Id = Guid.NewGuid(),
                Name = input.Name,
                User = input.UserName,
                Password = input.Password,
                Type = input.Type,
            };

            user = _context.Users.Add(user).Entity;
            await _context.SaveChangesAsync();

            return Ok(user.ToResource());
        }

        [HttpGet("Me")]
        public async Task<IActionResult> Me()
        {
            UserModel user = await _context.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null) return Forbid("Not authenticated");
            return Ok(user.ToResource());
        }

        private LoginOutput CreateToken(UserResource user)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("Type", user.Type.ToString()),
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expiration = DateTime.UtcNow.AddHours(36);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new LoginOutput
            {
                User = user,
                Token = "123456"//new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            };
        }

        public struct LoginInput
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public struct LoginOutput
        {
            public string Token { get; set; }
            public UserResource User { get; set; }
        }

        public struct UserStoreInput
        {
            public string Name { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public UserType Type { get; set; }
        }
    }
}