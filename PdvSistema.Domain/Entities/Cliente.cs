namespace PdvSistema.Domain.Entities;

public class Cliente
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = null!;
    public string? Documento { get; private set; } // CPF ou CNPJ
    public string? Telefone { get; private set; }
    public string? Email { get; private set; }
    public DateTime DataCadastro { get; private set; }

    protected Cliente() { } // EF Core

    public Cliente(string nome, string? documento, string? telefone, string? email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do cliente é obrigatório.");

        Nome = nome;
        Documento = documento;
        Telefone = telefone;
        Email = email;
        DataCadastro = DateTime.UtcNow;
    }

    public void AtualizarDados(string nome, string? telefone, string? email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do cliente é obrigatório.");

        Nome = nome;
        Telefone = telefone;
        Email = email;
    }
}