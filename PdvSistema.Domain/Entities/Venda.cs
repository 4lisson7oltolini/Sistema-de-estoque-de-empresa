namespace PdvSistema.Domain.Entities
{
    public enum FormaPagamento { Dinheiro, Cartao, Pix }

    public class Venda
    {
        public int Id { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public int? ClienteId { get; set; }
        public int UsuarioId { get; set; }
        public FormaPagamento FormaPagamento { get; set; }

        private readonly List<ItemVenda> _itens = new();
        public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();

        public decimal ValorTotal => _itens.Sum(i => i.Subtotal);

        public void AdicionarItem(Produto produto, int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero.");

            produto.BaixarEstoque(quantidade); // já lança exceção se não tiver estoque

            _itens.Add(new ItemVenda
            {
                ProdutoId = produto.Id,
                Produto = produto,
                Quantidade = quantidade,
                PrecoUnitario = produto.PrecoVenda
            });
        }
    }
}