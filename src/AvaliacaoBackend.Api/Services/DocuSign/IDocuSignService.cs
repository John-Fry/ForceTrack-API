using System.Text.Json.Nodes;

namespace AvaliacaoBackend.Api.Services.DocuSign;

public interface IDocuSignService
{
    Task<JsonNode?> CriarEnvelopeRascunhoAsync(JsonObject envelope, CancellationToken cancellationToken = default);
    Task<JsonNode?> EnviarEnvelopeAsync(string envelopeId, CancellationToken cancellationToken = default);
    Task<JsonNode?> ObterStatusEnvelopeAsync(string envelopeId, CancellationToken cancellationToken = default);
}
