using Demo_Backend.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Demo_Backend.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Product> _productsCollection;
        private readonly IMongoCollection<Category> _categoriesCollection;
        private readonly IMongoCollection<User> _usersCollection;

        public MongoDbService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("ProductDB");


            _productsCollection = database.GetCollection<Product>("Products");
            _categoriesCollection = database.GetCollection<Category>("Categories");
            _usersCollection = database.GetCollection<User>("Users");
        }

        // ================= Products =================
        public async Task<List<Product>> GetAllProductsAsync() =>
            await _productsCollection.Find(_ => true).ToListAsync();

        public async Task<Product?> GetProductByCodeAsync(string productCode) =>
            await _productsCollection.Find(p => p.ProductCode == productCode).FirstOrDefaultAsync();
        public async Task<List<Product>> GetProductsAsync(string? query = null)
        {
            var allProducts = await _productsCollection.Find(_ => true).ToListAsync();
            if (string.IsNullOrWhiteSpace(query)) return allProducts;

            query = query.ToLower();
            var filtered = allProducts.FindAll(p =>
                (p.ProductName?.ToLower().Contains(query) ?? false) ||
                (p.CategoryName?.ToLower().Contains(query) ?? false) ||
                (p.ProductCode?.ToLower().Contains(query) ?? false)
            );
            return filtered;
        }


        public async Task AddProductAsync(Product product) =>
            await _productsCollection.InsertOneAsync(product);

        public async Task UpdateProductAsync(Product product) =>
            await _productsCollection.ReplaceOneAsync(p => p.ProductCode == product.ProductCode, product);

        public async Task DeleteProductAsync(string productCode) =>
            await _productsCollection.DeleteOneAsync(p => p.ProductCode == productCode);

        // ================= Categories =================
        public async Task<Category?> GetCategoryByIdAsync(string id) =>
  await _categoriesCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        public async Task<List<Category>> GetCategoriesAsync() =>
            await _categoriesCollection.Find(_ => true).ToListAsync();

        public async Task AddCategoryAsync(Category category) =>
            await _categoriesCollection.InsertOneAsync(category);

        public async Task UpdateCategoryAsync(Category category) =>
            await _categoriesCollection.ReplaceOneAsync(c => c.Id == category.Id, category);

        public async Task<bool> DeleteCategoryAsync(string categoryId)
        {
            var isUsed = await _productsCollection.Find(p => p.CategoryName == categoryId).AnyAsync();
            if (isUsed) return false;

            await _categoriesCollection.DeleteOneAsync(c => c.Id == categoryId);
            return true;
        }

        // ================= Users =================
        public async Task<List<User>> GetUsersAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        public async Task<User?> GetUserByEmailAsync(string email) =>
            await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
       


        public async Task<bool> IsUserExistsAsync(string email) =>
            await _usersCollection.Find(u => u.Email == email).AnyAsync();

        public async Task AddUserAsync(User user)
        {
            if (!user.Password.StartsWith("$2a$") && !user.Password.StartsWith("$2b$"))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            await _usersCollection.InsertOneAsync(user);
        }

        public async Task<bool> ValidateUserAsync(string email, string enteredPassword)
        {
            var user = await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(enteredPassword, user.Password);
        }
        public async Task UpdateUserAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            await _usersCollection.ReplaceOneAsync(filter, user);
        }

    }
}
