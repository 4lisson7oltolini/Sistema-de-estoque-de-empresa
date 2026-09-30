namespace PdvSistema.Application.DTOs.Produto;

public class AtualizarProdutoRequest
{
    public string Nome { get; set; } = string.Empty;

    public string? CodigoBarras { get; set; }

    public decimal PrecoCusto { get; set; }

    public decimal PrecoVenda { get; set; }

    public int EstoqueMinimo { get; set; }

    public int CategoriaId { get; set; }
}