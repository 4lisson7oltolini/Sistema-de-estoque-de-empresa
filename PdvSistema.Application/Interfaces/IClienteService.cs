using PdvSistema.Application.DTOs.Cliente;

namespace PdvSistema.Application.Interfaces;

public interface IClienteService
{
    Task<List<ClienteResponse>> ListarAsync();
    Task<ClienteResponse?> ObterPorIdAsync(int id);
    Task<ClienteResponse> CriarAsync(CriarClienteRequest request);
    Task<bool> AtualizarAsync(int id, CriarClienteRequest request);
    Task<bool> RemoverAsync(int id);
}
