using Microsoft.AspNetCore.Mvc;
using Demo_Backend.Models;
using Demo_Backend.Services;
using System.Threading.Tasks;
using Demo_Backend.DTO;

namespace Demo_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        // Store theme in memory for runtime only
        private static string _currentTheme = "Light"; // default theme

        // GET: api/settings
        [HttpGet]
        public IActionResult GetSettings()
        {
            return Ok(new { PreferredTheme = _currentTheme });
        }

        // PUT: api/settings
        [HttpPut]
        public IActionResult UpdateSettings([FromBody] UpdateSettingsDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.PreferredTheme))
                return BadRequest("PreferredTheme is required.");

            // Only allow "Light" or "Dark"
            var theme = dto.PreferredTheme.Trim();
            if (theme != "Light" && theme != "Dark")
                return BadRequest("PreferredTheme must be 'Light' or 'Dark'.");

            _currentTheme = theme;

            return Ok(new { message = $"Theme updated to {_currentTheme}." });
        }
    }
}
    
