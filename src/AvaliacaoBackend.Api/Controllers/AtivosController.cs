using AvaliacaoBackend.Api.Models;
using AvaliacaoBackend.Api.Services.Force1;
using Microsoft.AspNetCore.Mvc;

namespace AvaliacaoBackend.Api.Controllers;

[ApiController]
[Route("ativos")]
public class AtivosController : ControllerBase
{
    private readonly IForce1Service _force1Service;

    public AtivosController(IForce1Service force1Service)
    {
        _force1Service = force1Service;
    }

    [HttpGet("amostra")]
    public async Task<ActionResult<IReadOnlyCollection<Ativo>>> ObterAmostra(
        [FromQuery] int pagina = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var ativos = await _force1Service.ObterComputadoresAsync(pagina, cancellationToken);
            return Ok(ativos.Take(10));
        }
        catch (Force1ApiException ex)
        {
            return CriarErroForce1(ex);
        }
    }

    [HttpGet("amostra-raw")]
    public async Task<IActionResult> ObterAmostraRaw(
        [FromQuery] int pagina = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var raw = await _force1Service.ObterComputadoresRawAsync(pagina, cancellationToken);
            return Ok(raw);
        }
        catch (Force1ApiException ex)
        {
            return CriarErroForce1(ex);
        }
    }

    [HttpGet("computadores-sem-comunicacao")]
    public async Task<ActionResult<IReadOnlyCollection<Ativo>>> ObterComputadoresSemComunicacao(
        [FromQuery] int dias = 60,
        CancellationToken cancellationToken = default)
    {
        if (dias <= 0)
        {
            return BadRequest("O parametro 'dias' precisa ser maior que zero.");
        }

        try
        {
            var ativos = await _force1Service.ObterComputadoresSemComunicacaoAsync(dias, cancellationToken);
            return Ok(ativos);
        }
        catch (Force1ApiException ex)
        {
            return CriarErroForce1(ex);
        }
    }

    private ObjectResult CriarErroForce1(Force1ApiException ex)
    {
        return StatusCode(StatusCodes.Status502BadGateway, new
        {
            mensagem = "A API externa Force1 recusou a requisicao. Verifique credenciais, enterprise e formato de autenticacao exigido pela documentacao.",
            statusExterno = (int)ex.StatusCode,
            respostaExterna = ex.ResponseBody
        });
    }
}
