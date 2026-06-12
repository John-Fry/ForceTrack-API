using AvaliacaoBackend.Api.Models;
using AvaliacaoBackend.Api.Services.Clientes;
using Microsoft.AspNetCore.Mvc;

namespace AvaliacaoBackend.Api.Controllers;

[ApiController]
[Route("clientes")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clientes;

    public ClientesController(IClienteService clientes)
    {
        _clientes = clientes;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<Cliente>>> ObterTodos(CancellationToken cancellationToken)
    {
        return Ok(await _clientes.ObterTodosAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<Cliente>> Adicionar(Cliente cliente, CancellationToken cancellationToken)
    {
        await _clientes.AdicionarAsync(cliente, cancellationToken);
        return CreatedAtAction(nameof(ObterTodos), new { id = cliente.Id }, cliente);
    }
}
