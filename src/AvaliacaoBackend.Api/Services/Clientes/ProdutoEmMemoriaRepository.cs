using AvaliacaoBackend.Api.Models;

namespace AvaliacaoBackend.Api.Services.Clientes;

public class ProdutoEmMemoriaRepository : IProdutoRepository
{
    private readonly List<Produto> _produtos =
    [
        new Produto { Id = 1, Nome = "Notebook", Preco = 4500m },
        new Produto { Id = 2, Nome = "Mouse", Preco = 120m }
    ];

    public IReadOnlyCollection<Produto> ObterTodos()
    {
        return _produtos.AsReadOnly();
    }

    public Produto Adicionar(Produto produto)
    {
        produto.Id = produto.Id <= 0 ? ProximoId() : produto.Id;
        _produtos.Add(produto);

        return produto;
    }

    private int ProximoId()
    {
        return _produtos.Count == 0 ? 1 : _produtos.Max(produto => produto.Id) + 1;
    }
}
