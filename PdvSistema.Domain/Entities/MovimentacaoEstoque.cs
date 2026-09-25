using PdvSistema.Domain.Enums;
namespace PdvSistema.Domain.Entities;
public class MovimentacaoEstoque
{
    public int Id { get; private set; }
    public int ProdutoId { get; private set; }
    public Produto Produto { get; private set; } = null!;
    public TipoMovimentacao Tipo { get; private set; }
    public int Quantidade { get; private set; }
    public string? Observacao { get; private set; }
    public DateTime Data { get; private set; }

    protected MovimentacaoEstoque() { }

    public MovimentacaoEstoque(Produto produto, TipoMovimentacao tipo, int quantidade, string? observacao = null)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade movimentada deve ser maior que zero.");

        Produto = produto ?? throw new ArgumentNullException(nameof(produto));
        ProdutoId = produto.Id;
        Tipo = tipo;
        Quantidade = quantidade;
        Observacao = observacao;
        Data = DateTime.UtcNow;
    }
}