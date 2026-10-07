using System.ComponentModel.DataAnnotations;

namespace PdvSistema.Web.Models;

public sealed class LoginFormModel
{
    [Required(ErrorMessage = "Informe seu e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe sua senha.")]
    public string Senha { get; set; } = string.Empty;
}

public sealed class BootstrapFormModel
{
    [Required(ErrorMessage = "Informe a chave de configuração.")]
    public string ChaveConfiguracao { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe seu nome.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe seu e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe uma senha.")]
    [MinLength(12, ErrorMessage = "Use pelo menos 12 caracteres.")]
    [MaxLength(128, ErrorMessage = "Use no máximo 128 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}

public sealed class LoginResultDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public AuthUsuarioDto Usuario { get; set; } = new();
}

public sealed class AuthUsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Perfil { get; set; }
    public bool Ativo { get; set; }
}

public sealed record ProtectedAuthSession(string AccessToken, DateTimeOffset ExpiresAt, AuthUsuarioDto Usuario);