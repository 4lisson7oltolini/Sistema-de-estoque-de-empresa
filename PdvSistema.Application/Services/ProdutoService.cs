using PdvSistema.Application.DTOs.Produto;
using PdvSistema.Application.Interfaces;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Interfaces;

namespace PdvSistema.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProdutoService(
        IProdutoRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ProdutoResponse>> ListarAsync()
    {
        var produtos = await _repository.ObterTodosAsync();

        return produtos
            .Select(Mapear)
            .ToList();
    }

    public async Task<ProdutoResponse?> ObterPorIdAsync(int id)
    {
        var produto =
            await _repository.ObterPorIdComCategoriaAsync(id);

        return produto is null
            ? null
            : Mapear(produto);
    }

    public async Task<ProdutoResponse> CriarAsync(
        CriarProdutoRequest request)
    {
        ValidarDados(request.Nome, request.PrecoCusto, request.PrecoVenda);

        var produto = new Produto
        {
            Nome = request.Nome.Trim(),
            CodigoBarras = string.IsNullOrWhiteSpace(request.CodigoBarras)
                ? null
                : request.CodigoBarras.Trim(),
            PrecoCusto = request.PrecoCusto,
            PrecoVenda = request.PrecoVenda,
            EstoqueMinimo = request.EstoqueMinimo,
            CategoriaId = request.CategoriaId
        };

        await _repository.AdicionarAsync(produto);

        await _unitOfWork.SaveChangesAsync();

        return Mapear(produto);
    }

    public async Task<bool> AtualizarAsync(
        int id,
        AtualizarProdutoRequest request)
    {
        ValidarDados(request.Nome, request.PrecoCusto, request.PrecoVenda);

        var produto = await _repository.ObterPorIdAsync(id);

        if (produto is null)
            return false;

        produto.Nome = request.Nome.Trim();

        produto.CodigoBarras =
            string.IsNullOrWhiteSpace(request.CodigoBarras)
                ? null
                : request.CodigoBarras.Trim();

        produto.PrecoCusto = request.PrecoCusto;
        produto.PrecoVenda = request.PrecoVenda;
        produto.EstoqueMinimo = request.EstoqueMinimo;
        produto.CategoriaId = request.CategoriaId;

        _repository.Atualizar(produto);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var produto = await _repository.ObterPorIdAsync(id);

        if (produto is null)
            return false;

        _repository.Remover(produto);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static ProdutoResponse Mapear(Produto produto)
    {
        return new ProdutoResponse
        {
            Id = produto.Id,
            Nome = produto.Nome,
            CodigoBarras = produto.CodigoBarras,
            PrecoCusto = produto.PrecoCusto,
            PrecoVenda = produto.PrecoVenda,
            QuantidadeEstoque = produto.QuantidadeEstoque,
            EstoqueMinimo = produto.EstoqueMinimo,
            CategoriaId = produto.CategoriaId,
            CategoriaNome = produto.Categoria?.Nome
        };
    }

    private static void ValidarDados(
        string nome,
        decimal precoCusto,
        decimal precoVenda)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException(
                "O nome do produto é obrigatório."
            );

        if (precoCusto < 0)
            throw new ArgumentException(
                "O preço de custo não pode ser negativo."
            );

        if (precoVenda < 0)
            throw new ArgumentException(
                "O preço de venda não pode ser negativo."
            );
    }
}