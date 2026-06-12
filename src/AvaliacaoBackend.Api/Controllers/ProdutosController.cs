using AvaliacaoBackend.Api.Models;
using AvaliacaoBackend.Api.Services.Clientes;
using Microsoft.AspNetCore.Mvc;

namespace AvaliacaoBackend.Api.Controllers;

[ApiController]
[Route("produtos")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _produtos;

    public ProdutosController(IProdutoRepository produtos)
    {
        _produtos = produtos;
    }

    [HttpGet]
    public ActionResult<IReadOnlyCollection<Produto>> ObterTodos()
    {
        return Ok(_produtos.ObterTodos());
    }

    [HttpPost]
    public ActionResult<Produto> Adicionar(Produto produto)
    {
        var produtoCriado = _produtos.Adicionar(produto);
        return CreatedAtAction(nameof(ObterTodos), new { id = produtoCriado.Id }, produtoCriado);
    }
}
