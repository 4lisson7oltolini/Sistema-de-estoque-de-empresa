using PdvSistema.Domain.Enums;

namespace PdvSistema.Application.DTOs.Usuario;

public class CriarUsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
}
