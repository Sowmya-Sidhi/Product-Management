/*
    MongoDbService.cs
    - Single data-access gateway to MongoDB for Products, Categories and Users.
    - Exposes async CRUD methods used by controllers (Get/Add/Update/Delete).
    - Current implementation creates a local MongoClient with hard-coded connection string.
        Consider moving to DI/config (appsettings) and injecting IMongoClient for testability.
    - Note: some methods currently pull full collections into memory for filtering.
*/
using Demo_Backend.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace Demo_Backend.Services
{
        public class MongoDbService : IMongoDbService
        {
                private readonly IMongoCollection<Product> _productsCollection;
                private readonly IMongoCollection<Category> _categoriesCollection;
                private readonly IMongoCollection<User> _usersCollection;
                private readonly Microsoft.Extensions.Logging.ILogger<MongoDbService> _logger;

        // Constructor now accepts IMongoClient and IConfiguration via DI.
        // This avoids hard-coded connection strings and makes the service testable.
        public MongoDbService(IMongoClient client, IConfiguration config, Microsoft.Extensions.Logging.ILogger<MongoDbService> logger)
        {
            _logger = logger;
            var dbName = config["Mongo:Database"] ?? "ProductDB";
            var database = client.GetDatabase(dbName);

            _productsCollection = database.GetCollection<Product>("Products");
            _categoriesCollection = database.GetCollection<Category>("Categories");
            _usersCollection = database.GetCollection<User>("Users");
        }

        // ================= Products =================
        public async Task<List<Product>> GetAllProductsAsync(CancellationToken ct = default) =>
            await _productsCollection.Find(_ => true).ToListAsync(ct);

        public async Task<Product?> GetProductByCodeAsync(string productCode, CancellationToken ct = default) =>
            await _productsCollection.Find(p => p.ProductCode == productCode).FirstOrDefaultAsync(ct);

        // Search with server-side filtering + pagination
        public async Task<List<Product>> GetProductsAsync(string? query = null, int skip = 0, int limit = 50, CancellationToken ct = default)
        {
            var filter = Builders<Product>.Filter.Empty;
            if (!string.IsNullOrWhiteSpace(query))
            {
                var regex = new BsonRegularExpression(Regex.Escape(query), "i");
                filter = Builders<Product>.Filter.Or(
                    Builders<Product>.Filter.Regex(p => p.ProductName, regex),
                    Builders<Product>.Filter.Regex(p => p.CategoryName, regex),
                    Builders<Product>.Filter.Regex(p => p.ProductCode, regex)
                );
            }

            _logger?.LogDebug("Querying products with filter. Query={Query} Skip={Skip} Limit={Limit}", query, skip, limit);
            return await _productsCollection.Find(filter)
                                            .Skip(skip)
                                            .Limit(limit)
                                            .ToListAsync(ct);
        }


        public async Task AddProductAsync(Product product, CancellationToken ct = default) =>
            await _productsCollection.InsertOneAsync(product, cancellationToken: ct);

        public async Task UpdateProductAsync(string code, Product product, CancellationToken ct = default)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.ProductCode, code);
            await _productsCollection.ReplaceOneAsync(filter, product, cancellationToken: ct);
        }


        public async Task DeleteProductAsync(string productCode, CancellationToken ct = default) =>
            await _productsCollection.DeleteOneAsync(p => p.ProductCode == productCode, cancellationToken: ct);

        // ================= Categories =================
        public async Task<Category?> GetCategoryByIdAsync(string id, CancellationToken ct = default) =>
            await _categoriesCollection.Find(c => c.Id == id).FirstOrDefaultAsync(ct);

        public async Task<List<Category>> GetCategoriesAsync(CancellationToken ct = default) =>
            await _categoriesCollection.Find(_ => true).ToListAsync(ct);

        public async Task AddCategoryAsync(Category category, CancellationToken ct = default) =>
            await _categoriesCollection.InsertOneAsync(category, cancellationToken: ct);

        public async Task UpdateCategoryAsync(Category category, CancellationToken ct = default) =>
            await _categoriesCollection.ReplaceOneAsync(c => c.Id == category.Id, category, cancellationToken: ct);

        public async Task<bool> DeleteCategoryAsync(string categoryId, CancellationToken ct = default)
        {
            var isUsed = await _productsCollection.Find(p => p.CategoryName == categoryId).AnyAsync(ct);
            if (isUsed) return false;

            await _categoriesCollection.DeleteOneAsync(c => c.Id == categoryId, cancellationToken: ct);
            return true;
        }

        // ================= Users =================
        public async Task<List<User>> GetUsersAsync(CancellationToken ct = default) =>
            await _usersCollection.Find(_ => true).ToListAsync(ct);

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default) =>
            await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync(ct);

        public async Task<bool> IsUserExistsAsync(string email, CancellationToken ct = default) =>
            await _usersCollection.Find(u => u.Email == email).AnyAsync(ct);

        public async Task AddUserAsync(User user, CancellationToken ct = default)
        {
            // Password must be hashed by the authentication layer (AuthService) before insertion.
            await _usersCollection.InsertOneAsync(user, cancellationToken: ct);
        }

        public async Task UpdateUserAsync(User user, CancellationToken ct = default)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            await _usersCollection.ReplaceOneAsync(filter, user, cancellationToken: ct);
        }

    }
}
