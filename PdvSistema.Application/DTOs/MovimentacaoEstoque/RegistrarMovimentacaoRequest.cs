using PdvSistema.Domain.Enums;

namespace PdvSistema.Application.DTOs.MovimentacaoEstoque;

public class RegistrarMovimentacaoRequest
{
    public int ProdutoId { get; set; }

    public TipoMovimentacao Tipo { get; set; }

    public int Quantidade { get; set; }

    public string? Observacao { get; set; }
}