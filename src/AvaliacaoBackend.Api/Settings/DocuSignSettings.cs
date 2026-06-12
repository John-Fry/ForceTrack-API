namespace AvaliacaoBackend.Api.Settings;

public class DocuSignSettings
{
    public const string SectionName = "DocuSign";

    public string BaseUrl { get; set; } = "https://demo.docusign.net/restapi";
    public string AccountId { get; set; } = "";
    public string AccessToken { get; set; } = "";
}
