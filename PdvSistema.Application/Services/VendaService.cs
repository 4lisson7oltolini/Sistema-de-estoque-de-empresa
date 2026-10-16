using PdvSistema.Application.DTOs.Venda;
using PdvSistema.Application.Interfaces;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Enums;
using PdvSistema.Domain.Interfaces;

namespace PdvSistema.Application.Services;

public class VendaService : IVendaService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IVendaRepository _vendaRepository;
    private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VendaService(
        IProdutoRepository produtoRepository,
        IVendaRepository vendaRepository,
        IMovimentacaoEstoqueRepository movimentacaoRepository,
        IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _vendaRepository = vendaRepository;
        _movimentacaoRepository = movimentacaoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<VendaResponse>> ListarAsync()
    {
        var vendas = await _vendaRepository.ObterTodosAsync();
        return vendas.Select(Mapear).ToList();
    }

    public async Task<VendaResponse?> ObterPorIdAsync(int id)
    {
        var venda = await _vendaRepository.ObterPorIdComItensAsync(id);
        return venda is null ? null : Mapear(venda);
    }

    public async Task<VendaResponse> CriarAsync(CriarVendaRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.UsuarioId <= 0)
            throw new ArgumentException("O usuário da venda é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.FormaPagamento))
            throw new ArgumentException("A forma de pagamento é obrigatória.");

        if (request.Itens is null || request.Itens.Count == 0)
            throw new ArgumentException("A venda deve conter pelo menos um item.");

        var venda = new Venda
        {
            ClienteId = request.ClienteId,
            UsuarioId = request.UsuarioId,
            FormaPagamento = request.FormaPagamento.Trim()
        };
        var movimentacoes = new List<MovimentacaoEstoque>();

        foreach (var item in request.Itens)
        {
            if (item.Quantidade <= 0)
                throw new ArgumentException("A quantidade do item deve ser maior que zero.");

            var produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
            if (produto is null)
                throw new InvalidOperationException($"Produto {item.ProdutoId} não encontrado.");

            venda.AdicionarItem(produto, item.Quantidade);
            movimentacoes.Add(new MovimentacaoEstoque(produto, TipoMovimentacao.Saida, item.Quantidade, "Venda realizada"));
        }

        venda.Concluir();

        await _vendaRepository.AdicionarAsync(venda);
        foreach (var movimentacao in movimentacoes)
            await _movimentacaoRepository.AdicionarAsync(movimentacao);
        await _unitOfWork.SaveChangesAsync();

        return Mapear(venda);
    }

    public async Task<bool> CancelarAsync(int id)
    {
        var venda = await _vendaRepository.ObterPorIdComItensAsync(id);
        if (venda is null || venda.Status == "Cancelada")
            return false;

        foreach (var item in venda.Itens)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
            if (produto is not null)
            {
                produto.AdicionarEstoque(item.Quantidade);
                await _movimentacaoRepository.AdicionarAsync(new MovimentacaoEstoque(
                    produto,
                    TipoMovimentacao.Entrada,
                    item.Quantidade,
                    "Cancelamento de venda"));
            }
        }

        venda.Cancelar();
        _vendaRepository.Atualizar(venda);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static VendaResponse Mapear(Venda venda)
    {
        var itens = venda.Itens.Select(item => new ItemVendaResponse
        {
            Id = item.Id,
            ProdutoId = item.ProdutoId,
            ProdutoNome = item.Produto?.Nome ?? string.Empty,
            Quantidade = item.Quantidade,
            PrecoUnitario = item.PrecoUnitario,
            Subtotal = item.Subtotal
        }).ToList();

        return new VendaResponse
        {
            Id = venda.Id,
            Data = venda.Data,
            ClienteId = venda.ClienteId,
            UsuarioId = venda.UsuarioId,
            FormaPagamento = venda.FormaPagamento,
            Status = venda.Status,
            ValorTotal = venda.ValorTotal,
            Itens = itens
        };
    }
}
