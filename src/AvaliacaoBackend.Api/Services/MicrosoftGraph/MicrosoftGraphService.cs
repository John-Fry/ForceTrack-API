using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using AvaliacaoBackend.Api.Settings;
using Microsoft.Extensions.Options;

namespace AvaliacaoBackend.Api.Services.MicrosoftGraph;

public class MicrosoftGraphService : IMicrosoftGraphService
{
    private readonly HttpClient _httpClient;
    private readonly MicrosoftGraphSettings _settings;

    public MicrosoftGraphService(HttpClient httpClient, IOptions<MicrosoftGraphSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
    }

    public Task<JsonNode?> ObterUsuarioAtualAsync(CancellationToken cancellationToken = default)
    {
        return EnviarAsync(HttpMethod.Get, "/me", null, cancellationToken);
    }

    public Task<JsonNode?> ListarMensagensAsync(int quantidade, CancellationToken cancellationToken = default)
    {
        return EnviarAsync(HttpMethod.Get, $"/me/messages?$top={quantidade}", null, cancellationToken);
    }

    public Task<JsonNode?> CriarEventoCalendarioAsync(JsonObject evento, CancellationToken cancellationToken = default)
    {
        return EnviarAsync(HttpMethod.Post, "/me/events", evento, cancellationToken);
    }

    private async Task<JsonNode?> EnviarAsync(
        HttpMethod method,
        string url,
        JsonObject? body,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.AccessToken);

        if (body is not null)
        {
            request.Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        return string.IsNullOrWhiteSpace(json) ? null : JsonNode.Parse(json);
    }
}
