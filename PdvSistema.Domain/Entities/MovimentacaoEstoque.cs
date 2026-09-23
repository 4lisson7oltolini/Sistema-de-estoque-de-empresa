namespace PdvSistema.Domain.Entities;

public class MovimentacaoEstoque
{
    public int Id { get; set; }

    public int ProdutoId { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    public decimal PrecoUnitario { get; set; }

    public string? Observacao { get; set; }

    public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;
}