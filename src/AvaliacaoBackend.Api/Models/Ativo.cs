namespace AvaliacaoBackend.Api.Models;

public class Ativo
{
    public string? Id { get; set; }
    public string? Nome { get; set; }
    public string? Tipo { get; set; }
    public string? Cidade { get; set; }
    public DateTime? UltimaComunicacao { get; set; }
}
