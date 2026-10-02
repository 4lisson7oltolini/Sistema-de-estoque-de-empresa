using PdvSistema.Domain.Enums;

namespace PdvSistema.Application.DTOs.MovimentacaoEstoque;

public class MovimentacaoEstoqueResponse
{
    public int Id { get; set; }

    public int ProdutoId { get; set; }

    public string ProdutoNome { get; set; } = string.Empty;

    public TipoMovimentacao Tipo { get; set; }

    public int Quantidade { get; set; }

    public string? Observacao { get; set; }

    public DateTime Data { get; set; }
}