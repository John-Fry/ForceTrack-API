namespace AvaliacaoBackend.Api.Settings;

public class MicrosoftGraphSettings
{
    public const string SectionName = "MicrosoftGraph";

    public string BaseUrl { get; set; } = "https://graph.microsoft.com/v1.0";
    public string AccessToken { get; set; } = "";
}
