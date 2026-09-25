using PdvSistema.Domain.Entities;
namespace PdvSistema.Domain.Interfaces;

public interface IVendaRepository : IRepository<Venda>
{
    Task<Venda?> ObterPorIdComItensAsync(int id);
    Task<List<Venda>> ObterPorClienteAsync(int clienteId);
}