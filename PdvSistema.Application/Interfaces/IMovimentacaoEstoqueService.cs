using PdvSistema.Application.DTOs.MovimentacaoEstoque;

namespace PdvSistema.Application.Interfaces;

public interface IMovimentacaoEstoqueService
{
    Task<List<MovimentacaoEstoqueResponse>> ListarAsync(int? produtoId);
    Task<MovimentacaoEstoqueResponse?> ObterPorIdAsync(int id);
    Task<MovimentacaoEstoqueResponse?> RegistrarAsync(RegistrarMovimentacaoRequest request);
}