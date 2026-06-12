using AvaliacaoBackend.Api.Models;

namespace AvaliacaoBackend.Api.Services.Clientes;

public interface IClienteService
{
    List<Cliente> ObterTodos();
    void Adicionar(Cliente cliente);
    Task<List<Cliente>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default);
}
