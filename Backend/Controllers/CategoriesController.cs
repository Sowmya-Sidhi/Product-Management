using Microsoft.AspNetCore.Mvc;
using Demo_Backend.Models;
using Demo_Backend.Services;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace Demo_Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
   
    public class CategoriesController : ControllerBase
    {
        private readonly MongoDbService _mongoService;
        private readonly AuditService _auditService;

        public CategoriesController(MongoDbService mongoService, AuditService auditService)
        {
            _mongoService = mongoService;
            _auditService = auditService;
        }
       

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _mongoService.GetCategoriesAsync();
            return Ok(categories);
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(string id)
        {
            var category = await _mongoService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        // POST: api/categories
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return BadRequest("Category name cannot be empty.");

            // Optional: check for duplicate name
            var existing = (await _mongoService.GetCategoriesAsync())
                            .FirstOrDefault(c => c.Name.ToLower() == category.Name.ToLower());
            if (existing != null)
                return BadRequest("Category name already exists.");

            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
               ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            category.CreatedBy = userId;
            category.UpdatedBy = userId;

            await _mongoService.AddCategoryAsync(category);
            _auditService.LogAction(
               userId,
               "Created Category",
               $"Category Name: {category.Name}"
           );

            return Ok(category);
        }

        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] Category category)
        {
            var existing = await _mongoService.GetCategoryByIdAsync(id);
            if (existing == null) return NotFound();

            if (string.IsNullOrWhiteSpace(category.Name))
                return BadRequest("Category name cannot be empty.");

        var products = await _mongoService.GetProductsAsync();
        bool isUsed = products.Any(p => p.CategoryName == existing.Name);

        if (isUsed && !string.Equals(existing.Name, category.Name, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Cannot update category name. It is currently used by existing products.");
            }



            existing.Name = category.Name;
            category.CreatedAt = existing.CreatedAt;
            category.CreatedBy = existing.CreatedBy;
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
             ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            category.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = userId;

            await _mongoService.UpdateCategoryAsync(existing);
            _auditService.LogAction(
               userId,
               "Updated Category",
               $"Category Name: {category.Name}"
           );

            return Ok(existing);
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var existing = await _mongoService.GetCategoryByIdAsync(id);
            if (existing == null) return NotFound();

            // Optional: check if category is used by any products
            var products = await _mongoService.GetProductsAsync();
            if (products.Any(p => p.CategoryName == existing.Name))
                return BadRequest("Cannot delete. This category is used by products.");
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
             ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _mongoService.DeleteCategoryAsync(id);
            _auditService.LogAction(
               userId,
               "Deleted Category",
               $"Category Name: {existing.Name}"
           );

            return Ok($"Category '{existing.Name}' deleted.");
        }
    }
}
