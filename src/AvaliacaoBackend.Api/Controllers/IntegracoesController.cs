using System.Text.Json.Nodes;
using AvaliacaoBackend.Api.Services.DocuSign;
using AvaliacaoBackend.Api.Services.GoogleMaps;
using AvaliacaoBackend.Api.Services.MicrosoftGraph;
using Microsoft.AspNetCore.Mvc;

namespace AvaliacaoBackend.Api.Controllers;

[ApiController]
[Route("integracoes")]
public class IntegracoesController : ControllerBase
{
    private readonly IGoogleMapsService _googleMaps;
    private readonly IDocuSignService _docuSign;
    private readonly IMicrosoftGraphService _microsoftGraph;

    public IntegracoesController(
        IGoogleMapsService googleMaps,
        IDocuSignService docuSign,
        IMicrosoftGraphService microsoftGraph)
    {
        _googleMaps = googleMaps;
        _docuSign = docuSign;
        _microsoftGraph = microsoftGraph;
    }

    [HttpGet("google-maps/geocode")]
    public Task<JsonNode?> Geocodificar([FromQuery] string endereco, CancellationToken cancellationToken)
    {
        return _googleMaps.GeocodificarEnderecoAsync(endereco, cancellationToken);
    }

    [HttpGet("google-maps/reverse-geocode")]
    public Task<JsonNode?> ReverterCoordenadas(
        [FromQuery] decimal latitude,
        [FromQuery] decimal longitude,
        CancellationToken cancellationToken)
    {
        return _googleMaps.ReverterCoordenadasAsync(latitude, longitude, cancellationToken);
    }

    [HttpGet("google-maps/validar-endereco")]
    public Task<bool> ValidarEndereco([FromQuery] string endereco, CancellationToken cancellationToken)
    {
        return _googleMaps.ValidarEnderecoAsync(endereco, cancellationToken);
    }

    [HttpPost("docusign/envelopes")]
    public Task<JsonNode?> CriarEnvelope([FromBody] JsonObject envelope, CancellationToken cancellationToken)
    {
        return _docuSign.CriarEnvelopeRascunhoAsync(envelope, cancellationToken);
    }

    [HttpPost("docusign/envelopes/{envelopeId}/send")]
    public Task<JsonNode?> EnviarEnvelope(string envelopeId, CancellationToken cancellationToken)
    {
        return _docuSign.EnviarEnvelopeAsync(envelopeId, cancellationToken);
    }

    [HttpGet("docusign/envelopes/{envelopeId}")]
    public Task<JsonNode?> ObterStatusEnvelope(string envelopeId, CancellationToken cancellationToken)
    {
        return _docuSign.ObterStatusEnvelopeAsync(envelopeId, cancellationToken);
    }

    [HttpGet("graph/me")]
    public Task<JsonNode?> ObterUsuarioAtual(CancellationToken cancellationToken)
    {
        return _microsoftGraph.ObterUsuarioAtualAsync(cancellationToken);
    }

    [HttpGet("graph/messages")]
    public Task<JsonNode?> ListarMensagens([FromQuery] int quantidade = 10, CancellationToken cancellationToken = default)
    {
        return _microsoftGraph.ListarMensagensAsync(quantidade, cancellationToken);
    }

    [HttpPost("graph/events")]
    public Task<JsonNode?> CriarEventoCalendario([FromBody] JsonObject evento, CancellationToken cancellationToken)
    {
        return _microsoftGraph.CriarEventoCalendarioAsync(evento, cancellationToken);
    }
}
