using AvaliacaoBackend.Api.Models;
using System.Text.Json.Nodes;

namespace AvaliacaoBackend.Api.Services.Force1;

public interface IForce1Service
{
    Task<IReadOnlyCollection<Ativo>> ObterComputadoresAsync(
        int pagina = 0,
        CancellationToken cancellationToken = default);

    Task<JsonNode?> ObterComputadoresRawAsync(
        int pagina = 0,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Ativo>> ObterComputadoresSemComunicacaoAsync(
        int diasSemComunicacao,
        CancellationToken cancellationToken = default);
}
