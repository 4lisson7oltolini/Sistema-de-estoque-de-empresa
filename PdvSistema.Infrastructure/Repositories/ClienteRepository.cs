using Microsoft.EntityFrameworkCore;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Interfaces;
using PdvSistema.Infrastructure.Data;

namespace PdvSistema.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly PdvSistemaDbContext _context;

    public ClienteRepository(PdvSistemaDbContext context) => _context = context;

    public async Task<Cliente?> ObterPorIdAsync(int id) =>
        await _context.Clientes.FindAsync(id);

    public async Task<List<Cliente>> ObterTodosAsync() =>
        await _context.Clientes.OrderBy(cliente => cliente.Nome).ToListAsync();

    public async Task AdicionarAsync(Cliente entidade) =>
        await _context.Clientes.AddAsync(entidade);

    public void Atualizar(Cliente entidade) => _context.Clientes.Update(entidade);

    public void Remover(Cliente entidade) => _context.Clientes.Remove(entidade);
}
