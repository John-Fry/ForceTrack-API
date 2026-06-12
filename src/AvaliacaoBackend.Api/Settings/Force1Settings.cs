namespace AvaliacaoBackend.Api.Settings;

public class Force1Settings
{
    public const string SectionName = "Force1";

    public string BaseUrl { get; set; } = "https://api.magma-3.com";
    public string AssetsPath { get; set; } = "/v2/Force1/GetAssets";
    public string LoginPath { get; set; } = "/v2/Auth/Login";
    public string ComputerAssetType { get; set; } = "computador";
    public string Login { get; set; } = "";
    public string Senha { get; set; } = "";
    public string Enterprise { get; set; } = "";
}
