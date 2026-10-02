using PdvSistema.Application.DTOs.MovimentacaoEstoque;
using PdvSistema.Application.Interfaces;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Enums;
using PdvSistema.Domain.Interfaces;

namespace PdvSistema.Application.Services;

public class MovimentacaoEstoqueService : IMovimentacaoEstoqueService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MovimentacaoEstoqueService(
        IProdutoRepository produtoRepository,
        IMovimentacaoEstoqueRepository movimentacaoRepository,
        IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _movimentacaoRepository = movimentacaoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<MovimentacaoEstoqueResponse>> ListarAsync(int? produtoId)
    {
        var movimentacoes = await _movimentacaoRepository.ListarAsync(produtoId);
        return movimentacoes.Select(Mapear).ToList();
    }

    public async Task<MovimentacaoEstoqueResponse?> ObterPorIdAsync(int id)
    {
        var movimentacao = await _movimentacaoRepository.ObterPorIdAsync(id);
        return movimentacao is null ? null : Mapear(movimentacao);
    }

    public async Task<MovimentacaoEstoqueResponse?> RegistrarAsync(
        RegistrarMovimentacaoRequest request)
    {
        if (!Enum.IsDefined(request.Tipo))
            throw new ArgumentException("Tipo de movimentação inválido.");
        if (request.Quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        var produto = await _produtoRepository.ObterPorIdAsync(request.ProdutoId);
        if (produto is null)
            return null;

        if (request.Tipo == TipoMovimentacao.Entrada)
            produto.AdicionarEstoque(request.Quantidade);
        else
            produto.BaixarEstoque(request.Quantidade);

        var movimentacao = new MovimentacaoEstoque(
            produto,
            request.Tipo,
            request.Quantidade,
            string.IsNullOrWhiteSpace(request.Observacao)
                ? null
                : request.Observacao.Trim());

        await _movimentacaoRepository.AdicionarAsync(movimentacao);
        await _unitOfWork.SaveChangesAsync();
        return Mapear(movimentacao);
    }

    private static MovimentacaoEstoqueResponse Mapear(MovimentacaoEstoque movimentacao) => new()
    {
        Id = movimentacao.Id,
        ProdutoId = movimentacao.ProdutoId,
        ProdutoNome = movimentacao.Produto.Nome,
        Tipo = movimentacao.Tipo,
        Quantidade = movimentacao.Quantidade,
        Observacao = movimentacao.Observacao,
        Data = movimentacao.Data
    };
}