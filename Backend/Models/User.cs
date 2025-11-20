/*
  User.cs
  - Domain model persisted in MongoDB `Users` collection.
  - Stores email, hashed password and role. `Id` is a MongoDB ObjectId represented as string.
  - Be careful: never return the `Password` field in API responses; map to DTOs instead.
*/
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Demo_Backend.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("Password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("Role")]
        public string Role { get; set; } = string.Empty;
    }
}
