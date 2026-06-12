namespace AvaliacaoBackend.Api.Models;

public class Produto
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public decimal Preco { get; set; }
}
