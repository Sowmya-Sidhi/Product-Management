/*
  AuthController.cs
  - Exposes authentication endpoints: login, register, get current user and list users.
  - Uses `MongoDbService` to read/write user records and `JwtService` to create tokens.
  - Verifies passwords using BCrypt and returns a JWT on successful login.
*/
using Demo_Backend.DTO;
using Demo_Backend.Models;
using Demo_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

//Namespaces group related classes to avoid name conflicts.
namespace Demo_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //AuthController handles user authentication and registration.
    public class AuthController : ControllerBase
    {
        private readonly IMongoDbService _mongoService;
        private readonly JwtService _jwtService;
        private readonly IAuthService _authService;

        public AuthController(IMongoDbService mongoService, JwtService jwtService, IAuthService authService)
        {
            _mongoService = mongoService;
            _jwtService = jwtService;
            _authService = authService;
        }

        // POST: api/auth/login 
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequesDto loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
                return BadRequest("Email and Password are required.");

            var user = await _mongoService.GetUserByEmailAsync(loginRequest.Email);

            if (user == null || !await _authService.VerifyPasswordAsync(loginRequest.Password, user.Password))
                return Unauthorized("Invalid email or password.");

            var userId = user.Id?.ToString();
            if (string.IsNullOrWhiteSpace(userId))
                return StatusCode(500, "User identifier is missing for this account.");

            var token = _jwtService.GenerateToken(userId, user.Email, user.Role);
            var response = new LoginResponseDto
            {
                Id = userId,
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
                // Hash password using AuthService before saving
                Password = await _authService.HashPasswordAsync(registerRequest.Password),
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
            // Validate email parameter
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            // Fetch user by email
            var user = await _mongoService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound($"No user found with email: {email}");
            
            // Return user details excluding sensitive info
            var response = new
            {
                Email = user.Email,
                Role = user.Role
            };

            return Ok(response);
        }

    }
}
