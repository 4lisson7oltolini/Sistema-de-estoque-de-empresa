using PdvSistema.Application.DTOs.Produto;
using PdvSistema.Application.Services;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Interfaces;

namespace PdvSistema.Tests;

public class ProdutoServiceTests
{
    [Fact]
    public async Task CriarAsync_DeveNormalizarNomeECodigoBarrasESalvar()
    {
        var repository = new FakeProdutoRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new ProdutoService(repository, unitOfWork);

        var response = await service.CriarAsync(new CriarProdutoRequest
        {
            Nome = "  Cafe  ",
            CodigoBarras = "  12345  ",
            PrecoCusto = 4.50m,
            PrecoVenda = 6.25m,
            EstoqueMinimo = 3,
            CategoriaId = 1
        });

        Assert.Equal("Cafe", response.Nome);
        Assert.Equal("12345", response.CodigoBarras);
        Assert.Equal(1, response.Id);
        Assert.Equal(1, unitOfWork.SaveCount);
        Assert.Single(repository.Produtos);
    }

    [Theory]
    [InlineData("", 1, 2)]
    [InlineData("Produto", -1, 2)]
    [InlineData("Produto", 1, -1)]
    public async Task CriarAsync_DeveRejeitarDadosInvalidos(
        string nome,
        decimal precoCusto,
        decimal precoVenda)
    {
        var repository = new FakeProdutoRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new ProdutoService(repository, unitOfWork);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CriarAsync(new CriarProdutoRequest
            {
                Nome = nome,
                PrecoCusto = precoCusto,
                PrecoVenda = precoVenda
            }));

        Assert.Empty(repository.Produtos);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarFalseQuandoProdutoNaoExiste()
    {
        var unitOfWork = new FakeUnitOfWork();
        var service = new ProdutoService(new FakeProdutoRepository(), unitOfWork);

        var atualizado = await service.AtualizarAsync(42, new AtualizarProdutoRequest
        {
            Nome = "Produto",
            PrecoCusto = 1,
            PrecoVenda = 2
        });

        Assert.False(atualizado);
        Assert.Equal(0, unitOfWork.SaveCount);
    }

    private sealed class FakeProdutoRepository : IProdutoRepository
    {
        public List<Produto> Produtos { get; } = [];

        public Task<Produto?> ObterPorIdAsync(int id) =>
            Task.FromResult(Produtos.FirstOrDefault(produto => produto.Id == id));

        public Task<Produto?> ObterPorIdComCategoriaAsync(int id) =>
            ObterPorIdAsync(id);

        public Task<List<Produto>> ObterTodosAsync() =>
            Task.FromResult(Produtos);

        public Task<List<Produto>> ObterPorCategoriaAsync(int categoriaId) =>
            Task.FromResult(Produtos.Where(produto => produto.CategoriaId == categoriaId).ToList());

        public Task AdicionarAsync(Produto entidade)
        {
            entidade.Id = Produtos.Count + 1;
            Produtos.Add(entidade);
            return Task.CompletedTask;
        }

        public void Atualizar(Produto entidade)
        {
        }

        public void Remover(Produto entidade) => Produtos.Remove(entidade);
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
