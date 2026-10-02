using PdvSistema.Domain.Enums;

namespace PdvSistema.Application.DTOs.Usuario;

public class UsuarioResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
}
