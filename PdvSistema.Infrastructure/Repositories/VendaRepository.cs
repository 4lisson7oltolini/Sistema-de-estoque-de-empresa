using Microsoft.EntityFrameworkCore;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Interfaces;
using PdvSistema.Infrastructure.Data;

namespace PdvSistema.Infrastructure.Repositories;

public class VendaRepository : IVendaRepository
{
    private readonly PdvSistemaDbContext _context;

    public VendaRepository(PdvSistemaDbContext context) => _context = context;

    public async Task<Venda?> ObterPorIdAsync(int id) =>
        await _context.Vendas
            .Include(venda => venda.Cliente)
            .FirstOrDefaultAsync(venda => venda.Id == id);

    public async Task<Venda?> ObterPorIdComItensAsync(int id) =>
        await _context.Vendas
            .Include(venda => venda.Cliente)
            .Include(venda => venda.Itens)
            .ThenInclude(item => item.Produto)
            .FirstOrDefaultAsync(venda => venda.Id == id);

    public async Task<List<Venda>> ObterTodosAsync() =>
        await _context.Vendas
            .Include(venda => venda.Cliente)
            .Include(venda => venda.Itens)
            .ThenInclude(item => item.Produto)
            .OrderByDescending(venda => venda.Data)
            .ToListAsync();

    public async Task<List<Venda>> ObterPorClienteAsync(int clienteId) =>
        await _context.Vendas
            .Include(venda => venda.Cliente)
            .Include(venda => venda.Itens)
            .ThenInclude(item => item.Produto)
            .Where(venda => venda.ClienteId == clienteId)
            .OrderByDescending(venda => venda.Data)
            .ToListAsync();

    public async Task AdicionarAsync(Venda entidade) =>
        await _context.Vendas.AddAsync(entidade);

    public void Atualizar(Venda entidade) => _context.Vendas.Update(entidade);

    public void Remover(Venda entidade) => _context.Vendas.Remove(entidade);
}
