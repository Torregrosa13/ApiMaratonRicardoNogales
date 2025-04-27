using ApiMaratonRicardoNogales.Data;
using ApiMaratonRicardoNogales.DTOs;
using ApiMaratonRicardoNogales.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NugetMaraton;
using System.Security.Cryptography;
using System.Text;

namespace ApiMaratonRicardoNogales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MaratonContext context;
        private readonly IConfiguration config;

        public AuthController(MaratonContext context, IConfiguration config)
        {
            this.context = context;
            this.config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var user = new Usuario
            {
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Rol = dto.Rol
            };

            context.Usuarios.Add(user);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginDTO dto)
        {
            var user = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || user.PasswordHash != HashPassword(dto.Password))
            {
                return Unauthorized();
            }

            var token = JwtHelper.GenerateJwtToken(user, config);

            return new LoginResponseDTO { Token = token };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}