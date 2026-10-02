namespace PdvSistema.Application.DTOs.Cliente;

public class CriarClienteRequest
{
    public string Nome { get; set; } = string.Empty;
    public string? Documento { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
}
