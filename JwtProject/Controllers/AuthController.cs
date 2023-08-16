using JwtProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login(Login model)
        {
            if (model.Username == "admin" && model.Password == "1234")
            {
                var tokenResult = GenerateToken(model.Username);

                return Ok(tokenResult);
            }

            return BadRequest("Kullanıcı adı veya şifre yanlış!");
        }

        private LoginResult GenerateToken(string username)
        {
            var claims = GenerateUserClaims(username);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.Now.AddMinutes(10);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: expiry, signingCredentials: signIn);

            LoginResult loginResult = new()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiry = expiry
            };

            return loginResult;
        }
        private Claim[] GenerateUserClaims(string username)
        {
            return new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Username", username),
                    new Claim("soyad", "ffff"),
               };
        }
    }
}
