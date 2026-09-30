namespace PdvSistema.Domain.Entities;

public class Produto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? CodigoBarras { get; set; }

    public decimal PrecoCusto { get; set; }

    public decimal PrecoVenda { get; set; }

    public int QuantidadeEstoque { get; private set; }

    public int EstoqueMinimo { get; set; }

    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    public void AdicionarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException(
                "A quantidade deve ser maior que zero.",
                nameof(quantidade)
            );

        QuantidadeEstoque += quantidade;
    }

    public void BaixarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException(
                "A quantidade deve ser maior que zero.",
                nameof(quantidade)
            );

        if (quantidade > QuantidadeEstoque)
            throw new InvalidOperationException(
                "Estoque insuficiente."
            );

        QuantidadeEstoque -= quantidade;
    }
}