namespace PdvSistema.Application.DTOs.Venda;

public class VendaResponse
{
    public int Id { get; set; }

    public DateTime Data { get; set; }

    public int? ClienteId { get; set; }

    public int UsuarioId { get; set; }

    public string FormaPagamento { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public decimal ValorTotal { get; set; }

    public List<ItemVendaResponse> Itens { get; set; } = [];
}
