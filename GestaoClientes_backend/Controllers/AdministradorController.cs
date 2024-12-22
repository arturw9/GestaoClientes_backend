using GestaoClientes_backend.ViewsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestaoClientes_backend.Controllers
{
    public class AdministradorController : MainControllerApi
    {
        private readonly IConfiguration _configuration;

        public AdministradorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("/Administrador/Login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            if (model.Username == _configuration["Jwt:Username"].ToString() && model.Password == _configuration["Jwt:Password"].ToString())
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.Username) }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new { Token = tokenHandler.WriteToken(token) });
            }

            return Unauthorized();
        }
    }
}