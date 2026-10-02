using System.ComponentModel.DataAnnotations;

namespace PdvSistema.Web.Models;

public sealed class ProdutoDto
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

public sealed class CategoriaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public sealed class ProdutoFormModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do produto.")]
    [StringLength(120, ErrorMessage = "Use no máximo 120 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    public string? CodigoBarras { get; set; }

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "O custo não pode ser negativo.")]
    public decimal PrecoCusto { get; set; }

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "O preço não pode ser negativo.")]
    public decimal PrecoVenda { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O estoque mínimo não pode ser negativo.")]
    public int EstoqueMinimo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria.")]
    public int CategoriaId { get; set; }
}

public sealed class CategoriaFormModel
{
    [Required(ErrorMessage = "Informe o nome da categoria.")]
    [StringLength(80, ErrorMessage = "Use no máximo 80 caracteres.")]
    public string Nome { get; set; } = string.Empty;
}