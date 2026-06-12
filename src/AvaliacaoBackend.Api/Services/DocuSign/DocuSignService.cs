using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using AvaliacaoBackend.Api.Settings;
using Microsoft.Extensions.Options;

namespace AvaliacaoBackend.Api.Services.DocuSign;

public class DocuSignService : IDocuSignService
{
    private readonly HttpClient _httpClient;
    private readonly DocuSignSettings _settings;

    public DocuSignService(HttpClient httpClient, IOptions<DocuSignSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
    }

    public Task<JsonNode?> CriarEnvelopeRascunhoAsync(JsonObject envelope, CancellationToken cancellationToken = default)
    {
        envelope["status"] = "created";
        return EnviarJsonAsync(HttpMethod.Post, CaminhoEnvelopes(), envelope, cancellationToken);
    }

    public Task<JsonNode?> EnviarEnvelopeAsync(string envelopeId, CancellationToken cancellationToken = default)
    {
        var body = new JsonObject { ["status"] = "sent" };
        return EnviarJsonAsync(HttpMethod.Put, $"{CaminhoEnvelopes()}/{Uri.EscapeDataString(envelopeId)}", body, cancellationToken);
    }

    public Task<JsonNode?> ObterStatusEnvelopeAsync(string envelopeId, CancellationToken cancellationToken = default)
    {
        return EnviarJsonAsync(HttpMethod.Get, $"{CaminhoEnvelopes()}/{Uri.EscapeDataString(envelopeId)}", null, cancellationToken);
    }

    private async Task<JsonNode?> EnviarJsonAsync(
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

    private string CaminhoEnvelopes()
    {
        return $"/v2.1/accounts/{Uri.EscapeDataString(_settings.AccountId)}/envelopes";
    }
}
