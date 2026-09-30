using PdvSistema.Application.DTOs.Produto;

namespace PdvSistema.Application.Interfaces;

public interface IProdutoService
{
    Task<List<ProdutoResponse>> ListarAsync();

    Task<ProdutoResponse?> ObterPorIdAsync(int id);

    Task<ProdutoResponse> CriarAsync(CriarProdutoRequest request);

    Task<bool> AtualizarAsync(
        int id,
        AtualizarProdutoRequest request);

    Task<bool> RemoverAsync(int id);
}