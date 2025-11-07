using Microsoft.AspNetCore.Mvc;
using Demo_Backend.Models;
using Demo_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace Demo_Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly MongoDbService _mongoService;
        private readonly AuditService _auditService;

        public ProductsController(MongoDbService mongoService, AuditService auditService)
        {
            _mongoService = mongoService;
            _auditService = auditService;
        }
      

        // GET: api/products

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {

            var products = await _mongoService.GetProductsAsync();
            return Ok(products);
        }

        // GET: api/products/{code}
        [HttpGet("{code}")]
        
        public async Task<IActionResult> GetProduct(string code)
        {
            var product = await _mongoService.GetProductByCodeAsync(code);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            // Check for duplicate code
            var existing = await _mongoService.GetProductByCodeAsync(product.ProductCode);
            if (existing != null)
                return BadRequest("Product code already exists.");

            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
              ?? User.FindFirstValue(ClaimTypes.NameIdentifier);// will now be correct
            product.CreatedBy = userId;
            product.UpdatedBy = userId;



            await _mongoService.AddProductAsync(product);
            _auditService.LogAction(
             userId,
              "Created Product",
              $"Product Name: {product.ProductName}"
          );
            return Ok(product);
        }

        // PUT: api/products/{code}
       [HttpPut("{code}")]
[Authorize(Roles = "Admin,Manager")]
public async Task<IActionResult> UpdateProduct(string code, [FromBody] Product product)
{
    var existing = await _mongoService.GetProductByCodeAsync(code);
    if (existing == null) return NotFound();

    // Keep original created info
    product.Id = existing.Id;
    product.CreatedAt = existing.CreatedAt;
    product.CreatedBy = existing.CreatedBy;

    var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
              ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

    product.UpdatedAt = DateTime.UtcNow;
    product.UpdatedBy = userId;

    // ✅ now passing old code separately
    await _mongoService.UpdateProductAsync(code, product);

    _auditService.LogAction(
        userId,
        "Updated Product",
        $"Product Name: {product.ProductName}"
    );

    return Ok(product);
}


        // DELETE: api/products/{code}
        [HttpDelete("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(string code)
        {
            var existing = await _mongoService.GetProductByCodeAsync(code);
            if (existing == null) return NotFound();
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            await _mongoService.DeleteProductAsync(code);
            _auditService.LogAction(
           userId,
            "Updated Product",
            $"Product Name: {existing.ProductName}"
        );
            return Ok($"Product {existing.ProductName} deleted.");
        }

        // Optional: Search products by text
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            var allProducts = await _mongoService.GetProductsAsync();
            var filtered = allProducts.Where(p =>
                (p.ProductName?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (p.CategoryName?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (p.ProductCode?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();

            return Ok(filtered);
        }
    }
}
