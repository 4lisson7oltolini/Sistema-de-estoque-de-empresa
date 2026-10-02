namespace PdvSistema.Application.DTOs.Venda;

public class CriarVendaRequest
{
    public int? ClienteId { get; set; }

    public int UsuarioId { get; set; }

    public string FormaPagamento { get; set; } = string.Empty;

    public List<CriarItemVendaRequest> Itens { get; set; } = [];
}
