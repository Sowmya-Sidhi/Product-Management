using BCrypt.Net;
using Demo_Backend.DTO;
using Demo_Backend.Models;
using Demo_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Demo_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MongoDbService _mongoService;

        public UsersController(MongoDbService mongoService)
        {
            _mongoService = mongoService;
        }

        // POST: api/users/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("Email and new password are required.");

            var user = await _mongoService.GetUserByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("User not found.");

            // Hash the new password
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _mongoService.UpdateUserAsync(user);

            return Ok("Password reset successfully.");
        }
    }
}
