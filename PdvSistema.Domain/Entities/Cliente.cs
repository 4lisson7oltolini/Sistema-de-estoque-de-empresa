namespace PdvSistema.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string? Documento { get; set; } // CPF ou CNPJ
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public DateTime DataCadastro { get; set; }

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