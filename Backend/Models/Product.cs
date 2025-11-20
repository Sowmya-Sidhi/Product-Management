/*
  Product.cs
  - Domain model for products persisted in the `Products` collection in MongoDB.
  - Contains audit fields (`CreatedAt/UpdatedAt/CreatedBy/UpdatedBy`) which controllers set.
  - Note: `CategoryName` currently stores a category identifier or name depending on usage;
    confirm model semantics before changing delete/update logic.
*/
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Demo_Backend.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("ProductCode")]
        public string ProductCode { get; set; } = string.Empty;

        [BsonElement("ProductName")]
        public string ProductName { get; set; } = string.Empty;

       

        [BsonElement("Price")]
        public decimal Price { get; set; }

        [BsonElement("IsActive")]
        public bool IsActive { get; set; }

       

        [BsonElement("CategoryName")]
        public string CategoryName { get; set; } = string.Empty;

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("CreatedBy")]
        public string CreatedBy { get; set; } = string.Empty;

        [BsonElement("UpdatedBy")]
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
