using AvaliacaoBackend.Api.Models;
using AvaliacaoBackend.Api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AvaliacaoBackend.Api.Services.Clientes;

public class ClienteMongoService : IClienteService
{
    private readonly IMongoCollection<Cliente> _clientes;

    public ClienteMongoService(IOptions<MongoSettings> options)
    {
        var settings = options.Value;
        var mongoClient = new MongoClient(settings.ConnectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);

        _clientes = database.GetCollection<Cliente>(settings.ClientesCollection);
    }

    public List<Cliente> ObterTodos()
    {
        return _clientes.Find(_ => true).ToList();
    }

    public void Adicionar(Cliente cliente)
    {
        _clientes.InsertOne(cliente);
    }

    public Task<List<Cliente>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return _clientes.Find(_ => true).ToListAsync(cancellationToken);
    }

    public Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        return _clientes.InsertOneAsync(cliente, cancellationToken: cancellationToken);
    }
}
