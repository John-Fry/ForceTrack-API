using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AvaliacaoBackend.Api.Models;

public class Cliente
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public required string Nome { get; set; }

    public required string Email { get; set; }
}
