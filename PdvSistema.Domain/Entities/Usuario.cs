namespace PdvSistema.Domain.Entities;
using PdvSistema.Domain.Enums;
public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string SenhaHash { get; set; } = null!;
    public PerfilUsuario Perfil { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }

    protected Usuario() { }

    public Usuario(string nome, string email, string senhaHash, PerfilUsuario perfil)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O e-mail é obrigatório.");

        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        Perfil = perfil;
        Ativo = true;
        DataCadastro = DateTime.UtcNow;
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}