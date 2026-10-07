using PdvSistema.Application.DTOs.Usuario;

namespace PdvSistema.Application.Interfaces;

public interface IUsuarioService
{
    Task<List<UsuarioResponse>> ListarAsync();
    Task<UsuarioResponse?> ObterPorIdAsync(int id);
    Task<UsuarioResponse> CriarAsync(CriarUsuarioRequest request);
    Task<bool> AtualizarAsync(int id, AtualizarUsuarioRequest request);
    Task<bool> RemoverAsync(int id);
}
