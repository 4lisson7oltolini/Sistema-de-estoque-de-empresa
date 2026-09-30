namespace PdvSistema.Domain.Entities;

public class Venda
{
    public int Id { get; set; }

    public DateTime Data { get; set; } = DateTime.Now;

    public int? ClienteId { get; set; }

    public Cliente? Cliente { get; set; }

    public int UsuarioId { get; set; }

    public string FormaPagamento { get; set; } = string.Empty;

    private readonly List<ItemVenda> _itens = new();

    public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();

    public decimal ValorTotal => _itens.Sum(i => i.Subtotal);

    public string Status { get; private set; } = "Pendente";

    public void AdicionarItem(Produto produto, int quantidade)
    {
        if (quantidade <= 0)
        {
            throw new ArgumentException(
                "Quantidade deve ser maior que zero.",
                nameof(quantidade)
            );
        }

        produto.BaixarEstoque(quantidade);

        _itens.Add(new ItemVenda
        {
            ProdutoId = produto.Id,
            Produto = produto,
            Quantidade = quantidade,
            PrecoUnitario = produto.PrecoVenda
        });
    }

    public void Concluir()
    {
        Status = "Concluida";
    }

    public void Cancelar()
    {
        Status = "Cancelada";
    }
}