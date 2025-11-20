/*
  DashboardController.cs
  - Provides lightweight dashboard endpoints (summary statistics) by aggregating product/category counts.
  - Uses `MongoDbService` to retrieve lists and computes simple counts returned in `DashboardSummaryDto`.
  - Keep this controller small — expensive aggregations should be moved to database-side queries/indexes if needed.
*/
using Demo_Backend.DTO;
using Demo_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Demo_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IMongoDbService _mongoService;

        public DashboardController(IMongoDbService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var products = await _mongoService.GetAllProductsAsync();
            var categories = await _mongoService.GetCategoriesAsync();

            var summary = new DashboardSummaryDto
            {
                TotalProducts = products.Count,
                TotalActiveProducts = products.Count(p => p.IsActive),
                TotalCategories = categories.Count
            };

            return Ok(summary);
        }
    }
}
