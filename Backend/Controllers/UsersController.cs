/*
  UsersController.cs
  - User-related API endpoints (example: password reset).
  - Uses `IMongoDbService` to locate and update user records.
  - Password hashing and verification are delegated to `IAuthService`.
*/
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
        private readonly IMongoDbService _mongoService;
        private readonly IAuthService _authService;

        public UsersController(IMongoDbService mongoService, IAuthService authService)
        {
            _mongoService = mongoService;
            _authService = authService;
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

            // Hash the new password via AuthService
            user.Password = await _authService.HashPasswordAsync(dto.NewPassword);
            await _mongoService.UpdateUserAsync(user);

            return Ok(new { message = "Password reset successfully." });
        }
    }
}
