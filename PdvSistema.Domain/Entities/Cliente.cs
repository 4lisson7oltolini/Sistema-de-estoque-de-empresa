namespace PdvSistema.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Telefone { get; set; }

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}