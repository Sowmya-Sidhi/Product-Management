using BCrypt.Net;
using Demo_Backend.DTO;
using Demo_Backend.Models;
using Demo_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace Demo_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MongoDbService _mongoService;
        private readonly JwtService _jwtService;

        public AuthController(MongoDbService mongoService, JwtService jwtService)
        {
            _mongoService = mongoService;
            _jwtService = jwtService;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequesDto loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
                return BadRequest("Email and Password are required.");

            var user = await _mongoService.GetUserByEmailAsync(loginRequest.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
                return Unauthorized("Invalid email or password.");
            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Email, user.Role);
            var response = new LoginResponseDto
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                Role = user.Role,
                Token = token
            };

            return Ok(response);
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            if (string.IsNullOrWhiteSpace(registerRequest.Email) || string.IsNullOrWhiteSpace(registerRequest.Password))
                return BadRequest("Email and Password are required.");

            if (await _mongoService.IsUserExistsAsync(registerRequest.Email))
                return BadRequest("User with this email already exists.");

            var newUser = new User
            {

                Email = registerRequest.Email,
                Password = registerRequest.Password, // will be hashed in service
                Role = registerRequest.Role ?? "User"
            };

            await _mongoService.AddUserAsync(newUser);

            return Ok(new { Message = "User registered successfully." });
        }

        // Optional: GET: api/auth/users (admin only)
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _mongoService.GetUsersAsync();
           
            var list = users.Select(u => new { u.Email, u.Role }).ToList();
            return Ok(list);
        }
        // GET: api/auth/current?email=someone@example.com
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var user = await _mongoService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound($"No user found with email: {email}");

            var response = new
            {
               
                Email = user.Email,
                Role = user.Role
            };

            return Ok(response);
        }

    }
}
