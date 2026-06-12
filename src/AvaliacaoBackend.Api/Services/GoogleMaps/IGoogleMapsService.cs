using System.Text.Json.Nodes;

namespace AvaliacaoBackend.Api.Services.GoogleMaps;

public interface IGoogleMapsService
{
    Task<JsonNode?> GeocodificarEnderecoAsync(string endereco, CancellationToken cancellationToken = default);
    Task<JsonNode?> ReverterCoordenadasAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken = default);
    Task<bool> ValidarEnderecoAsync(string endereco, CancellationToken cancellationToken = default);
}
