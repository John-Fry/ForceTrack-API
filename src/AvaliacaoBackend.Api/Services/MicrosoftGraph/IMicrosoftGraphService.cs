using System.Text.Json.Nodes;

namespace AvaliacaoBackend.Api.Services.MicrosoftGraph;

public interface IMicrosoftGraphService
{
    Task<JsonNode?> ObterUsuarioAtualAsync(CancellationToken cancellationToken = default);
    Task<JsonNode?> ListarMensagensAsync(int quantidade, CancellationToken cancellationToken = default);
    Task<JsonNode?> CriarEventoCalendarioAsync(JsonObject evento, CancellationToken cancellationToken = default);
}
