using ApiWithAccessToken.Data;
using ApiWithAccessToken.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ApiWithAccessToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(AuthDbContext context,IConfiguration config) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(AppUserDTO user) 
        {
            if(user is null)
            {
                return BadRequest("Usuário Inválido");
            }
            var getUser  = await context.AppUsers.FirstOrDefaultAsync(u=> u.Email == user.Email);
            if (getUser is not null) 
            {
                return BadRequest("Usuário Inválido");
            }
            var entity = context.AppUsers.Add(new Models.AppUser()
            { 
                Name = user.Name,
                Email = user.Email,
                Password =BCrypt.Net.BCrypt.HashPassword(user.Password)
               
            }).Entity;
            await context.SaveChangesAsync();
            context.UserRoles.Add(new Models.UserRole()
            {
                AppUserId = entity.Id,
                Role = user.Role

            });
            await context.SaveChangesAsync();
            return Ok("Usuário cadastrado com sucesso");
        }

        [HttpPost("login/{email}/{password}")]
        public async Task<IActionResult> Login( string email, string password)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Dados inválidos");  
            }
            var user = await context.AppUsers.FirstOrDefaultAsync(u=> u.Email == email);
            if(user is null)
            {
                return BadRequest("Dados inválidos");
                
            }
            
            var verifyPassowrd = BCrypt.Net.BCrypt.Verify(password,user.Password);
            if (!verifyPassowrd)
            {
                return NotFound("Credenciais inválidas"); 
            }
            var getRole = await context.UserRoles.FirstOrDefaultAsync(r=> r.AppUserId == user.Id);
            string key = $"{config["Authentication:Key"]}.{user.Name}.{user.Id}";
            string accessToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
            return Ok($"Access Token: {accessToken}");
        }

        [HttpGet("protected-admin")]
        [Authorize(AuthenticationSchemes ="Custom-Scheme",Roles ="Admin")]
        public IActionResult AdminGetProtectedMessage() => Ok("Você está autenticado como Admin");
        
        [HttpGet("protected-user")]
        [Authorize(AuthenticationSchemes = "Custom-Scheme", Roles = "User")]
        public IActionResult UserGetProtectedMessageUser() => Ok("Você está autenticado como User");
    }
}
