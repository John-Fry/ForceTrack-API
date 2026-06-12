using System.Net;

namespace AvaliacaoBackend.Api.Services.Force1;

public class Force1ApiException : Exception
{
    public Force1ApiException(HttpStatusCode statusCode, string responseBody)
        : base($"A API Force1 retornou {(int)statusCode} ({statusCode}).")
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    public HttpStatusCode StatusCode { get; }
    public string ResponseBody { get; }
}
