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

public sealed class ClienteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Documento { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public DateTime DataCadastro { get; set; }
}

public sealed class MovimentacaoEstoqueDto
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Tipo { get; set; }
    public int Quantidade { get; set; }
    public string? Observacao { get; set; }
    public DateTime Data { get; set; }
}

public sealed class ItemVendaDto
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public sealed class VendaDto
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public int? ClienteId { get; set; }
    public int UsuarioId { get; set; }
    public string FormaPagamento { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public List<ItemVendaDto> Itens { get; set; } = [];
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

    [Range(0, int.MaxValue, ErrorMessage = "O saldo inicial não pode ser negativo.")]
    public int EstoqueInicial { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria.")]
    public int CategoriaId { get; set; }
}

public sealed class CategoriaFormModel
{
    [Required(ErrorMessage = "Informe o nome da categoria.")]
    [StringLength(80, ErrorMessage = "Use no máximo 80 caracteres.")]
    public string Nome { get; set; } = string.Empty;
}

public sealed class MovimentacaoEstoqueFormModel
{
    public int ProdutoId { get; set; }

    [Range(1, 2, ErrorMessage = "Selecione entrada ou saída.")]
    public int Tipo { get; set; } = 1;

    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantidade { get; set; } = 1;

    [StringLength(250, ErrorMessage = "Use no máximo 250 caracteres na observação.")]
    public string? Observacao { get; set; }
}

public sealed class ClienteFormModel
{
    [Required(ErrorMessage = "Informe o nome do cliente.")]
    [StringLength(120, ErrorMessage = "Use no máximo 120 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(30, ErrorMessage = "Use no máximo 30 caracteres no documento.")]
    public string? Documento { get; set; }

    [StringLength(30, ErrorMessage = "Use no máximo 30 caracteres no telefone.")]
    public string? Telefone { get; set; }

    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(254, ErrorMessage = "Use no máximo 254 caracteres no e-mail.")]
    public string? Email { get; set; }
}

public sealed class ItemVendaRequestModel
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
}

public sealed class VendaRequestModel
{
    public int? ClienteId { get; set; }
    public int UsuarioId { get; set; }
    public string FormaPagamento { get; set; } = string.Empty;
    public List<ItemVendaRequestModel> Itens { get; set; } = [];
}