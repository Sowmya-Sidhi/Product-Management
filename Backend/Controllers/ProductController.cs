/*
  ProductController.cs
  - Exposes Product CRUD endpoints used by the frontend (GET, POST, PUT, DELETE, search).
  - Endpoints are protected with `[Authorize]` and role-based checks for write operations.
  - Calls `MongoDbService` for persistence and `AuditService` to log user actions.
*/

//Importing necessary namespaces
using Microsoft.AspNetCore.Mvc;
using Demo_Backend.Models;
using Demo_Backend.Services;
using Demo_Backend.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

//Defining the namespace for the controller
namespace Demo_Backend.Controllers
{
    //Applying authorization and routing attributes to the controller
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    //Defining the ProductsController class which inherits from ControllerBase
    public class ProductsController : ControllerBase
    { 
        //Declaring private readonly fields for MongoDbService and AuditService
        private readonly IMongoDbService _mongoService;
        private readonly AuditService _auditService;

        //Constructor to initialize the services via dependency injection
        public ProductsController(IMongoDbService mongoService, AuditService auditService)
        {
            //Initializing the private fields with the injected services
            _mongoService = mongoService;
            _auditService = auditService;
        }


        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetProducts() //Endpoint to retrieve all products
        {
            //Calls the Service to get all products
            var products = await _mongoService.GetProductsAsync();

            //It prevents returning the entire Product model directly
            //by mapping to ProductDto
            //This avoids exposing sensitive/internal fields.
            //Mapping each Product to ProductDto 
            var dtos = products.Select(p => new ProductDto
            {
                Id = p.Id ?? string.Empty,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                CategoryName = p.CategoryName,
                Price = (double)p.Price,
                IsActive = p.IsActive
            }).ToList();
            //Returning the list of ProductDto objects as the response
            return Ok(dtos);
        }



    // GET: api/products/{code}
    [HttpGet("{code}")]//Endpoint to retrieve a product by its code
        public async Task<IActionResult> GetProduct(string code)
        {
            var product = await _mongoService.GetProductByCodeAsync(code);
            if (product == null) return NotFound();

            var dto = new ProductDto
            {
                Id = product.Id ?? string.Empty,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                CategoryName = product.CategoryName,
                Price = (double)product.Price,
                IsActive = product.IsActive
            };

            return Ok(dto);
        }

        // POST: api/products
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> AddProduct([FromBody] Product product) //Endpoint to add a new product
        {
            // Check for duplicate code
            var existing = await _mongoService.GetProductByCodeAsync(product.ProductCode);
            if (existing != null)
                return BadRequest("Product code already exists.");

            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
                        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                            ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty; // guard nulls
            product.CreatedBy = userId;
            product.UpdatedBy = userId;
            await _mongoService.AddProductAsync(product);
            _auditService.LogAction(
             userId,
              "Created Product",
              $"Product Name: {product.ProductName}"
          );

            var createdDto = new ProductDto
            {
                Id = product.Id ?? string.Empty,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                CategoryName = product.CategoryName,
                Price = (double)product.Price,
                IsActive = product.IsActive
            };

            return Ok(createdDto);
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
              ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    product.UpdatedAt = DateTime.UtcNow;
    product.UpdatedBy = userId;

    // ✅ now passing old code separately

    //Here using Update in order to preserve other fields
    await _mongoService.UpdateProductAsync(code, product);

    _auditService.LogAction(
        userId,
        "Updated Product",
        $"Product Name: {product.ProductName}"
    );

    var dto = new ProductDto
    {
        Id = product.Id ?? string.Empty,
        ProductCode = product.ProductCode,
        ProductName = product.ProductName,
        CategoryName = product.CategoryName,
        Price = (double)product.Price,
        IsActive = product.IsActive
    };

    return Ok(dto);
}


        // DELETE: api/products/{code}
        [HttpDelete("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(string code)
        {
            var existing = await _mongoService.GetProductByCodeAsync(code);
            if (existing == null) return NotFound();
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty;

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
            // Use server-side filtering in the service when available
            var products = await _mongoService.GetProductsAsync(query);
            var filtered = products.Select(p => new ProductDto
            {
                Id = p.Id ?? string.Empty,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                CategoryName = p.CategoryName,
                Price = (double)p.Price,
                IsActive = p.IsActive
            }).ToList();

            return Ok(filtered);
        }
    }
}
