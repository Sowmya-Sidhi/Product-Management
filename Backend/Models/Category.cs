using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Demo_Backend.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;

        // Audit fields
        [BsonElement("CreatedBy")]
        public string CreatedBy { get; set; } = string.Empty;
        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("UpdatedBy")]
        public string UpdatedBy { get; set; } = string.Empty;
        [BsonElement("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
