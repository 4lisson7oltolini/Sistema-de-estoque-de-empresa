namespace PdvSistema.Application.DTOs.Venda;

public class ItemVendaResponse
{
    public int Id { get; set; }

    public int ProdutoId { get; set; }

    public string ProdutoNome { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    public decimal PrecoUnitario { get; set; }

    public decimal Subtotal { get; set; }
}
