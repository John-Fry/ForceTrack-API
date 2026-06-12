namespace AvaliacaoBackend.Api.Settings;

public class MongoSettings
{
    public const string SectionName = "Mongo";

    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "avaliacao_backend";
    public string ClientesCollection { get; set; } = "clientes";
}
