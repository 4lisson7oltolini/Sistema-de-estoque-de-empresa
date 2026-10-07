using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using PdvSistema.Application.DTOs.Usuario;
using PdvSistema.Application.Interfaces;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Enums;
using PdvSistema.Domain.Interfaces;

namespace PdvSistema.Application.Services;

public sealed class AuthService(
    IUsuarioRepository usuarios,
    IUnitOfWork unitOfWork,
    IPasswordHasher<Usuario> passwordHasher) : IAuthService
{
    public async Task<UsuarioResponse?> AutenticarAsync(string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha) || senha.Length > 128)
            return null;

        var usuario = await usuarios.ObterPorEmailAsync(email.Trim().ToLowerInvariant());
        if (usuario is null || !usuario.Ativo)
            return null;

        var senhaValida = VerificarSenha(usuario, senha, out var precisaAtualizarHash);
        if (!senhaValida)
            return null;

        if (precisaAtualizarHash)
        {
            usuario.SenhaHash = passwordHasher.HashPassword(usuario, senha);
            usuarios.Atualizar(usuario);
            await unitOfWork.SaveChangesAsync();
        }

        return Mapear(usuario);
    }

    public async Task<UsuarioResponse> CriarAdministradorInicialAsync(string nome, string email, string senha)
    {
        if ((await usuarios.ObterTodosAsync()).Count > 0)
            throw new InvalidOperationException("A configuração inicial já foi concluída.");

        var nomeNormalizado = nome?.Trim();
        var emailNormalizado = NormalizarEmail(email);
        ValidarSenha(senha);

        if (string.IsNullOrWhiteSpace(nomeNormalizado))
            throw new ArgumentException("O nome é obrigatório.");

        var usuario = new Usuario(nomeNormalizado, emailNormalizado, string.Empty, PerfilUsuario.Administrador);
        usuario.SenhaHash = passwordHasher.HashPassword(usuario, senha);
        await usuarios.AdicionarAsync(usuario);
        await unitOfWork.SaveChangesAsync();
        return Mapear(usuario);
    }

    private bool VerificarSenha(Usuario usuario, string senha, out bool precisaAtualizarHash)
    {
        precisaAtualizarHash = false;

        if (EhHashLegado(usuario.SenhaHash))
        {
            var informado = SHA256.HashData(Encoding.UTF8.GetBytes(senha));
            var armazenado = Convert.FromHexString(usuario.SenhaHash);
            precisaAtualizarHash = CryptographicOperations.FixedTimeEquals(informado, armazenado);
            return precisaAtualizarHash;
        }

        var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha);
        precisaAtualizarHash = resultado == PasswordVerificationResult.SuccessRehashNeeded;
        return resultado is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }

    private static bool EhHashLegado(string hash) =>
        hash.Length == 64 && hash.All(Uri.IsHexDigit);

    private static string NormalizarEmail(string email)
    {
        var valor = email?.Trim();
        if (string.IsNullOrWhiteSpace(valor) || !MailAddress.TryCreate(valor, out _))
            throw new ArgumentException("Informe um e-mail válido.");

        return valor.ToLowerInvariant();
    }

    private static void ValidarSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 12 || senha.Length > 128)
            throw new ArgumentException("A senha deve conter entre 12 e 128 caracteres.");
    }

    private static UsuarioResponse Mapear(Usuario usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        Perfil = usuario.Perfil,
        Ativo = usuario.Ativo,
        DataCadastro = usuario.DataCadastro
    };
}