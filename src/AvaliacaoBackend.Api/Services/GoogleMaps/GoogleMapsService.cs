using System.Globalization;
using System.Text.Json.Nodes;
using AvaliacaoBackend.Api.Settings;
using Microsoft.Extensions.Options;

namespace AvaliacaoBackend.Api.Services.GoogleMaps;

public class GoogleMapsService : IGoogleMapsService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleMapsSettings _settings;

    public GoogleMapsService(HttpClient httpClient, IOptions<GoogleMapsSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _httpClient.BaseAddress = new Uri(NormalizarBaseUrl(_settings.BaseUrl));
    }

    public Task<JsonNode?> GeocodificarEnderecoAsync(string endereco, CancellationToken cancellationToken = default)
    {
        var url = $"geocode/json?address={Uri.EscapeDataString(endereco)}&key={Uri.EscapeDataString(_settings.ApiKey)}";
        return EnviarAsync(url, cancellationToken);
    }

    public Task<JsonNode?> ReverterCoordenadasAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken = default)
    {
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lng = longitude.ToString(CultureInfo.InvariantCulture);
        var url = $"geocode/json?latlng={lat},{lng}&key={Uri.EscapeDataString(_settings.ApiKey)}";

        return EnviarAsync(url, cancellationToken);
    }

    public async Task<bool> ValidarEnderecoAsync(string endereco, CancellationToken cancellationToken = default)
    {
        var resposta = await GeocodificarEnderecoAsync(endereco, cancellationToken);
        return string.Equals(resposta?["status"]?.ToString(), "OK", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<JsonNode?> EnviarAsync(string url, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonNode.Parse(json);
    }

    private static string NormalizarBaseUrl(string baseUrl)
    {
        return baseUrl.EndsWith('/') ? baseUrl : $"{baseUrl}/";
    }
}
