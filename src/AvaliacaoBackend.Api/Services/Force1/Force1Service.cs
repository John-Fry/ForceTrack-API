using System.Net.Http.Headers;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using AvaliacaoBackend.Api.Models;
using AvaliacaoBackend.Api.Settings;
using Microsoft.Extensions.Options;

namespace AvaliacaoBackend.Api.Services.Force1;

public class Force1Service : IForce1Service
{
    private readonly HttpClient _httpClient;
    private readonly Force1Settings _settings;

    public Force1Service(HttpClient httpClient, IOptions<Force1Settings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
    }

    public async Task<IReadOnlyCollection<Ativo>> ObterComputadoresSemComunicacaoAsync(
        int diasSemComunicacao,
        CancellationToken cancellationToken = default)
    {
        var ativos = await ObterComputadoresAsync(cancellationToken: cancellationToken);
        var limite = DateTime.UtcNow.AddDays(-diasSemComunicacao);

        return ativos
            .Where(ativo => EhComputador(ativo.Tipo) && ativo.UltimaComunicacao.HasValue)
            .Where(ativo => ativo.UltimaComunicacao!.Value.ToUniversalTime() < limite)
            .ToList();
    }

    public async Task<IReadOnlyCollection<Ativo>> ObterComputadoresAsync(
        int pagina = 0,
        CancellationToken cancellationToken = default)
    {
        var json = await ObterComputadoresJsonAsync(pagina, cancellationToken);
        return ExtrairAtivos(json);
    }

    public async Task<JsonNode?> ObterComputadoresRawAsync(
        int pagina = 0,
        CancellationToken cancellationToken = default)
    {
        var json = await ObterComputadoresJsonAsync(pagina, cancellationToken);
        return JsonNode.Parse(json);
    }

    private async Task<string> ObterComputadoresJsonAsync(
        int pagina,
        CancellationToken cancellationToken)
    {
        var token = await AutenticarAsync(cancellationToken);
        var url = $"{_settings.AssetsPath}?assetType={Uri.EscapeDataString(_settings.ComputerAssetType)}&pagination={pagina}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Force1ApiException(response.StatusCode, json);
        }

        return json;
    }

    private async Task<string> AutenticarAsync(CancellationToken cancellationToken)
    {
        var payload = new
        {
            _settings.Enterprise,
            _settings.Login,
            Password = _settings.Senha
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, _settings.LoginPath);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Force1ApiException(response.StatusCode, json);
        }

        var token = ExtrairToken(json);
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("A API Force1 autenticou, mas nao retornou um token em formato reconhecido.");
        }

        return token;
    }

    private static string? ExtrairToken(string json)
    {
        var root = JsonNode.Parse(json);

        return LerToken(root, "token", "Token", "accessToken", "AccessToken", "jwt", "Jwt", "bearerToken", "BearerToken");
    }

    private static string? LerToken(JsonNode? node, params string[] nomes)
    {
        if (node is null)
        {
            return null;
        }

        if (node is JsonValue)
        {
            return node.ToString();
        }

        if (node is JsonObject obj)
        {
            foreach (var nome in nomes)
            {
                if (obj.TryGetPropertyValue(nome, out var value) && value is not null)
                {
                    return value.ToString();
                }
            }

            foreach (var child in obj.Select(property => property.Value))
            {
                var token = LerToken(child, nomes);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    return token;
                }
            }
        }

        return null;
    }

    private static List<Ativo> ExtrairAtivos(string json)
    {
        var root = JsonNode.Parse(json);
        if (root is null)
        {
            return [];
        }

        var arrays = EncontrarArrays(root).Where(array => array.Count > 0);
        return arrays
            .SelectMany(array => array.OfType<JsonObject>())
            .Select(MapearAtivo)
            .Where(ativo => ativo.Nome is not null || ativo.Id is not null || ativo.Tipo is not null)
            .ToList();
    }

    private static IEnumerable<JsonArray> EncontrarArrays(JsonNode node)
    {
        if (node is JsonArray array)
        {
            yield return array;
        }

        if (node is not JsonObject obj)
        {
            yield break;
        }

        foreach (var child in obj.Select(property => property.Value).OfType<JsonNode>())
        {
            foreach (var arrayNode in EncontrarArrays(child))
            {
                yield return arrayNode;
            }
        }
    }

    private static Ativo MapearAtivo(JsonObject obj)
    {
        return new Ativo
        {
            Id = LerString(obj, "Id", "id", "_id", "AssetId", "assetId", "codigo"),
            Nome = LerString(obj, "Name", "nome", "name", "Hostname", "hostname", "ComputerName", "computerName", "descricao"),
            Tipo = LerString(obj, "Type", "tipo", "type", "AssetType", "assetType", "categoria"),
            Cidade = LerString(obj, "City", "cidade", "city"),
            UltimaComunicacao = LerData(
                obj,
                "LastCommunication",
                "ultimaComunicacao",
                "lastCommunication",
                "LastSeen",
                "lastSeen",
                "LastContact",
                "lastContact",
                "LastSync",
                "lastSync",
                "ManualUpdatingDate",
                "Agent.DataLastCommunication")
        };
    }

    private static string? LerString(JsonObject obj, params string[] nomes)
    {
        foreach (var nome in nomes)
        {
            if (obj.TryGetPropertyValue(nome, out var value))
            {
                return value?.ToString();
            }
        }

        return null;
    }

    private static DateTime? LerData(JsonObject obj, params string[] nomes)
    {
        var valor = LerString(obj, nomes);
        return DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var data)
            ? data
            : null;
    }

    private static bool EhComputador(string? tipo)
    {
        if (string.IsNullOrWhiteSpace(tipo))
        {
            return true;
        }

        return tipo.Contains("computador", StringComparison.OrdinalIgnoreCase)
            || tipo.Contains("computer", StringComparison.OrdinalIgnoreCase)
            || tipo.Contains("desktop", StringComparison.OrdinalIgnoreCase)
            || tipo.Contains("notebook", StringComparison.OrdinalIgnoreCase);
    }
}
