using PdvSistema.Application.DTOs.Venda;

namespace PdvSistema.Application.Interfaces;

public interface IVendaService
{
    Task<List<VendaResponse>> ListarAsync();
    Task<VendaResponse?> ObterPorIdAsync(int id);
    Task<VendaResponse> CriarAsync(CriarVendaRequest request);
    Task<bool> CancelarAsync(int id);
}
