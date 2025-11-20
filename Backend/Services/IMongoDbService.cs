using Demo_Backend.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo_Backend.Services
{
    /// <summary>
    /// Interface for MongoDbService to allow DI and unit testing.
    /// Implementations provide async CRUD operations for Products, Categories and Users.
    /// </summary>
    public interface IMongoDbService
    {
        // Products
        Task<List<Product>> GetAllProductsAsync(CancellationToken ct = default);
        Task<List<Product>> GetProductsAsync(string? query = null, int skip = 0, int limit = 50, CancellationToken ct = default);
        Task<Product?> GetProductByCodeAsync(string productCode, CancellationToken ct = default);
        Task AddProductAsync(Product product, CancellationToken ct = default);
        Task UpdateProductAsync(string code, Product product, CancellationToken ct = default);
        Task DeleteProductAsync(string productCode, CancellationToken ct = default);

        // Categories
        Task<Category?> GetCategoryByIdAsync(string id, CancellationToken ct = default);
        Task<List<Category>> GetCategoriesAsync(CancellationToken ct = default);
        Task AddCategoryAsync(Category category, CancellationToken ct = default);
        Task UpdateCategoryAsync(Category category, CancellationToken ct = default);
        Task<bool> DeleteCategoryAsync(string categoryId, CancellationToken ct = default);

        // Users
        Task<List<User>> GetUsersAsync(CancellationToken ct = default);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default);
        Task<bool> IsUserExistsAsync(string email, CancellationToken ct = default);
        Task AddUserAsync(User user, CancellationToken ct = default);
        Task UpdateUserAsync(User user, CancellationToken ct = default);
    }
}
