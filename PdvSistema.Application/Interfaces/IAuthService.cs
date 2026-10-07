using PdvSistema.Application.DTOs.Usuario;

namespace PdvSistema.Application.Interfaces;

public interface IAuthService
{
    Task<UsuarioResponse?> AutenticarAsync(string email, string senha);
    Task<UsuarioResponse> CriarAdministradorInicialAsync(string nome, string email, string senha);
}