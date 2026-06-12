using AvaliacaoBackend.Api.Models;

namespace AvaliacaoBackend.Api.Services.Clientes;

public interface IProdutoRepository
{
    IReadOnlyCollection<Produto> ObterTodos();
    Produto Adicionar(Produto produto);
}
