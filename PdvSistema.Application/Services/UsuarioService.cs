using System.Security.Cryptography;
using System.Text;
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

    public UsuarioService(IUsuarioRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
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

        var usuario = new Usuario(nome, email, HashSenha(senha), request.Perfil);
        await _repository.AdicionarAsync(usuario);
        await _unitOfWork.SaveChangesAsync();
        return Mapear(usuario);
    }

    public async Task<bool> AtualizarAsync(int id, CriarUsuarioRequest request)
    {
        var usuario = await _repository.ObterPorIdAsync(id);
        if (usuario is null)
            return false;

        var nome = ValidarNome(request.Nome);
        var email = ValidarEmail(request.Email);

        usuario.Ativar();
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
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("O e-mail do usuário é obrigatório.");

        return valor;
    }

    private static string ValidarSenha(string senha)
    {
        var valor = senha?.Trim();
        if (string.IsNullOrWhiteSpace(valor) || valor.Length < 6)
            throw new ArgumentException("A senha deve conter pelo menos 6 caracteres.");

        return valor;
    }

    private static string HashSenha(string senha)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
        return Convert.ToHexString(bytes);
    }

    private static UsuarioResponse Mapear(Usuario usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        SenhaHash = usuario.SenhaHash,
        Perfil = usuario.Perfil,
        Ativo = usuario.Ativo,
        DataCadastro = usuario.DataCadastro
    };
}
