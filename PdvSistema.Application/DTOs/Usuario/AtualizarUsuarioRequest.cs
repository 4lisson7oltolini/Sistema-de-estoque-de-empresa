using PdvSistema.Domain.Enums;

namespace PdvSistema.Application.DTOs.Usuario;

public class AtualizarUsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
}