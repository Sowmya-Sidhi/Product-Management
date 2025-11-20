/*
  CategoriesController.cs
  - Controller for CRUD operations on Categories (GET, POST, PUT, DELETE).
  - Protected by JWT `[Authorize]` attribute; role checks used for admin/manager actions.
  - Calls `MongoDbService` for data operations and `AuditService` to record changes.
  - Note: controller currently performs some in-memory checks (e.g., whether category is used by products).
*/
using Microsoft.AspNetCore.Mvc;
using Demo_Backend.Models;
using Demo_Backend.Services;
using Demo_Backend.DTO;
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
        private readonly IMongoDbService _mongoService;
        private readonly AuditService _auditService;

        public CategoriesController(IMongoDbService mongoService, AuditService auditService)
        {
            _mongoService = mongoService;
            _auditService = auditService;
        }
       

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _mongoService.GetCategoriesAsync();
            var dtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id ?? string.Empty,
                Name = c.Name
            }).ToList();

            return Ok(dtos);
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(string id)
        {
            var category = await _mongoService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            var dto = new CategoryDto
            {
                Id = category.Id ?? string.Empty,
                Name = category.Name
            };

            return Ok(dto);
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
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
                category.CreatedBy = userId;
                category.UpdatedBy = userId;

            await _mongoService.AddCategoryAsync(category);
            _auditService.LogAction(
               userId,
               "Created Category",
               $"Category Name: {category.Name}"
           );

            var createdDto = new CategoryDto
            {
                Id = category.Id ?? string.Empty,
                Name = category.Name
            };

            return Ok(createdDto);
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
             ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            category.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = userId;

            await _mongoService.UpdateCategoryAsync(existing);
            _auditService.LogAction(
               userId,
               "Updated Category",
               $"Category Name: {category.Name}"
           );

            var updatedDto = new CategoryDto
            {
                Id = existing.Id ?? string.Empty,
                Name = existing.Name
            };

            return Ok(updatedDto);
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
             ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
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
