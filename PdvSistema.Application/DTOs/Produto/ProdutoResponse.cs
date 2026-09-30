namespace PdvSistema.Application.DTOs.Produto;

public class ProdutoResponse
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? CodigoBarras { get; set; }

    public decimal PrecoCusto { get; set; }

    public decimal PrecoVenda { get; set; }

    public int QuantidadeEstoque { get; set; }

    public int EstoqueMinimo { get; set; }

    public int CategoriaId { get; set; }

    public string? CategoriaNome { get; set; }
}