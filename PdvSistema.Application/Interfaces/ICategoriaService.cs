using PdvSistema.Application.DTOs.Categoria;

namespace PdvSistema.Application.Interfaces;

public enum ResultadoRemocaoCategoria
{
    Removida,
    NaoEncontrada,
    PossuiProdutos
}

public interface ICategoriaService
{
    Task<List<CategoriaResponse>> ListarAsync();
    Task<CategoriaResponse?> ObterPorIdAsync(int id);
    Task<CategoriaResponse> CriarAsync(CriarCategoriaRequest request);
    Task<bool> AtualizarAsync(int id, CriarCategoriaRequest request);
    Task<ResultadoRemocaoCategoria> RemoverAsync(int id);
}