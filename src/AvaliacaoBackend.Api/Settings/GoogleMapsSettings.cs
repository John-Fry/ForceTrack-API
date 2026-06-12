namespace AvaliacaoBackend.Api.Settings;

public class GoogleMapsSettings
{
    public const string SectionName = "GoogleMaps";

    public string BaseUrl { get; set; } = "https://maps.googleapis.com/maps/api/";
    public string ApiKey { get; set; } = "";
}
