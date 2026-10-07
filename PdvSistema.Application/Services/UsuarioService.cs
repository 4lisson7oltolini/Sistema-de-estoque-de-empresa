using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using PdvSistema.Application.DTOs.Usuario;
using PdvSistema.Application.Interfaces;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Enums;
using PdvSistema.Domain.Interfaces;

namespace PdvSistema.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UsuarioService(
        IUsuarioRepository repository,
        IUnitOfWork unitOfWork,
        IPasswordHasher<Usuario> passwordHasher)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UsuarioResponse>> ListarAsync() =>
        (await _repository.ObterTodosAsync()).Select(Mapear).ToList();

    public async Task<UsuarioResponse?> ObterPorIdAsync(int id)
    {
        var usuario = await _repository.ObterPorIdAsync(id);
        return usuario is null ? null : Mapear(usuario);
    }

    public async Task<UsuarioResponse> CriarAsync(CriarUsuarioRequest request)
    {
        var nome = ValidarNome(request.Nome);
        var email = ValidarEmail(request.Email);
        var senha = ValidarSenha(request.Senha);

        if (await _repository.ObterPorEmailAsync(email) is not null)
            throw new InvalidOperationException("Já existe um usuário com este e-mail.");

        var usuario = new Usuario(nome, email, string.Empty, request.Perfil);
        usuario.SenhaHash = _passwordHasher.HashPassword(usuario, senha);
        await _repository.AdicionarAsync(usuario);
        await _unitOfWork.SaveChangesAsync();
        return Mapear(usuario);
    }

    public async Task<bool> AtualizarAsync(int id, AtualizarUsuarioRequest request)
    {
        var usuario = await _repository.ObterPorIdAsync(id);
        if (usuario is null)
            return false;

        var nome = ValidarNome(request.Nome);
        var email = ValidarEmail(request.Email);

        var usuarioExistente = await _repository.ObterPorEmailAsync(email);
        if (usuarioExistente is not null && usuarioExistente.Id != id)
            throw new InvalidOperationException("Já existe um usuário com este e-mail.");

        usuario.Nome = nome;
        usuario.Email = email;
        usuario.Perfil = request.Perfil;
        _repository.Atualizar(usuario);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var usuario = await _repository.ObterPorIdAsync(id);
        if (usuario is null)
            return false;

        usuario.Desativar();
        _repository.Atualizar(usuario);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static string ValidarNome(string nome)
    {
        var valor = nome?.Trim();
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("O nome do usuário é obrigatório.");

        return valor;
    }

    private static string ValidarEmail(string email)
    {
        var valor = email?.Trim();
        if (string.IsNullOrWhiteSpace(valor) || !MailAddress.TryCreate(valor, out _))
            throw new ArgumentException("Informe um e-mail válido.");

        return valor.ToLowerInvariant();
    }

    private static string ValidarSenha(string senha)
    {
        var valor = senha;
        if (string.IsNullOrWhiteSpace(valor) || valor.Length < 12 || valor.Length > 128)
            throw new ArgumentException("A senha deve conter entre 12 e 128 caracteres.");

        return valor;
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
