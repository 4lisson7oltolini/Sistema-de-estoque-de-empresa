using PdvSistema.Domain.Entities;

namespace PdvSistema.Domain.Interfaces;

public interface IMovimentacaoEstoqueRepository
{
    Task<MovimentacaoEstoque?> ObterPorIdAsync(int id);
    Task<List<MovimentacaoEstoque>> ListarAsync(int? produtoId);
    Task AdicionarAsync(MovimentacaoEstoque movimentacao);
}