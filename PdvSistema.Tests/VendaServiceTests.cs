using PdvSistema.Application.DTOs.Venda;
using PdvSistema.Application.Interfaces;
using PdvSistema.Application.Services;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Enums;
using PdvSistema.Domain.Interfaces;

namespace PdvSistema.Tests;

public class VendaServiceTests
{
    [Fact]
    public async Task CriarAsync_DeveRegistrarVendaEAtualizarEstoque()
    {
        var produto = new Produto
        {
            Id = 10,
            Nome = "Café",
            PrecoCusto = 3m,
            PrecoVenda = 7m,
            EstoqueMinimo = 2,
            CategoriaId = 1
        };
        produto.AdicionarEstoque(5);

        var repository = new FakeProdutoRepository(produto);
        var vendaRepository = new FakeVendaRepository();
        var movimentacaoRepository = new FakeMovimentacaoRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new VendaService(repository, vendaRepository, movimentacaoRepository, unitOfWork);

        var response = await service.CriarAsync(new CriarVendaRequest
        {
            ClienteId = 1,
            UsuarioId = 2,
            FormaPagamento = "Cartão",
            Itens =
            [
                new CriarItemVendaRequest { ProdutoId = 10, Quantidade = 2 }
            ]
        });

        Assert.Equal("Concluida", response.Status);
        Assert.Equal(3, produto.QuantidadeEstoque);
        Assert.Single(vendaRepository.Vendas);
        Assert.Single(movimentacaoRepository.Movimentacoes);
        Assert.Equal(TipoMovimentacao.Saida, movimentacaoRepository.Movimentacoes[0].Tipo);
        Assert.Equal(1, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task CriarAsync_DeveRejeitarVendaSemItens()
    {
        var repository = new FakeProdutoRepository();
        var vendaRepository = new FakeVendaRepository();
        var movimentacaoRepository = new FakeMovimentacaoRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new VendaService(repository, vendaRepository, movimentacaoRepository, unitOfWork);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CriarAsync(new CriarVendaRequest
        {
            UsuarioId = 2,
            FormaPagamento = "Dinheiro",
            Itens = []
        }));
    }

    [Fact]
    public async Task CriarAsync_DeveRejeitarQuandoEstoqueForInsuficiente()
    {
        var produto = new Produto
        {
            Id = 8,
            Nome = "Pão",
            PrecoCusto = 2m,
            PrecoVenda = 5m,
            EstoqueMinimo = 1,
            CategoriaId = 1
        };
        produto.AdicionarEstoque(2);

        var repository = new FakeProdutoRepository(produto);
        var vendaRepository = new FakeVendaRepository();
        var movimentacaoRepository = new FakeMovimentacaoRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new VendaService(repository, vendaRepository, movimentacaoRepository, unitOfWork);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CriarAsync(new CriarVendaRequest
        {
            UsuarioId = 2,
            FormaPagamento = "Dinheiro",
            Itens =
            [
                new CriarItemVendaRequest { ProdutoId = 8, Quantidade = 5 }
            ]
        }));
    }

    [Fact]
    public async Task CancelarAsync_DeveRestaurarEstoqueERegistrarEntrada()
    {
        var produto = new Produto
        {
            Id = 12,
            Nome = "Arroz",
            PrecoCusto = 8m,
            PrecoVenda = 12m,
            CategoriaId = 1
        };
        produto.AdicionarEstoque(5);

        var venda = new Venda { UsuarioId = 2, FormaPagamento = "Pix" };
        venda.AdicionarItem(produto, 2);
        venda.Concluir();

        var produtoRepository = new FakeProdutoRepository(produto);
        var vendaRepository = new FakeVendaRepository(venda);
        var movimentacaoRepository = new FakeMovimentacaoRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new VendaService(produtoRepository, vendaRepository, movimentacaoRepository, unitOfWork);

        var cancelada = await service.CancelarAsync(venda.Id);

        Assert.True(cancelada);
        Assert.Equal(5, produto.QuantidadeEstoque);
        Assert.Single(movimentacaoRepository.Movimentacoes);
        Assert.Equal(TipoMovimentacao.Entrada, movimentacaoRepository.Movimentacoes[0].Tipo);
        Assert.Equal(1, unitOfWork.SaveCount);
    }

    private sealed class FakeProdutoRepository : IProdutoRepository
    {
        private readonly List<Produto> _produtos;

        public FakeProdutoRepository(params Produto[] produtos) => _produtos = produtos.ToList();

        public Task<Produto?> ObterPorIdAsync(int id) => Task.FromResult(_produtos.FirstOrDefault(p => p.Id == id));

        public Task<Produto?> ObterPorIdComCategoriaAsync(int id) => Task.FromResult(_produtos.FirstOrDefault(p => p.Id == id));

        public Task<List<Produto>> ObterTodosAsync() => Task.FromResult(_produtos);

        public Task<List<Produto>> ObterPorCategoriaAsync(int categoriaId) =>
            Task.FromResult(_produtos.Where(p => p.CategoriaId == categoriaId).ToList());

        public Task AdicionarAsync(Produto entidade)
        {
            _produtos.Add(entidade);
            return Task.CompletedTask;
        }

        public void Atualizar(Produto entidade) { }

        public void Remover(Produto entidade) => _produtos.Remove(entidade);
    }

    private sealed class FakeVendaRepository : IVendaRepository
    {
        public List<Venda> Vendas { get; } = [];

        public FakeVendaRepository(params Venda[] vendas) => Vendas.AddRange(vendas);

        public Task<Venda?> ObterPorIdAsync(int id) => Task.FromResult(Vendas.FirstOrDefault(v => v.Id == id));

        public Task<List<Venda>> ObterTodosAsync() => Task.FromResult(Vendas);

        public Task AdicionarAsync(Venda entidade)
        {
            entidade.Id = Vendas.Count + 1;
            Vendas.Add(entidade);
            return Task.CompletedTask;
        }

        public void Atualizar(Venda entidade) { }

        public void Remover(Venda entidade) => Vendas.Remove(entidade);

        public Task<Venda?> ObterPorIdComItensAsync(int id) => Task.FromResult(Vendas.FirstOrDefault(v => v.Id == id));

        public Task<List<Venda>> ObterPorClienteAsync(int clienteId) =>
            Task.FromResult(Vendas.Where(v => v.ClienteId == clienteId).ToList());
    }

    private sealed class FakeMovimentacaoRepository : IMovimentacaoEstoqueRepository
    {
        public List<MovimentacaoEstoque> Movimentacoes { get; } = [];

        public Task<MovimentacaoEstoque?> ObterPorIdAsync(int id) =>
            Task.FromResult(Movimentacoes.FirstOrDefault(movimentacao => movimentacao.Id == id));

        public Task<List<MovimentacaoEstoque>> ListarAsync(int? produtoId) =>
            Task.FromResult(produtoId is null
                ? Movimentacoes
                : Movimentacoes.Where(movimentacao => movimentacao.ProdutoId == produtoId).ToList());

        public Task AdicionarAsync(MovimentacaoEstoque movimentacao)
        {
            Movimentacoes.Add(movimentacao);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveCount++;
            return Task.FromResult(1);
        }
    }
}
