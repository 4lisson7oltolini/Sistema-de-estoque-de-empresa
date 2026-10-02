using Microsoft.EntityFrameworkCore;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Interfaces;
using PdvSistema.Infrastructure.Data;

namespace PdvSistema.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly PdvSistemaDbContext _context;

    public UsuarioRepository(PdvSistemaDbContext context) => _context = context;

    public async Task<Usuario?> ObterPorIdAsync(int id) =>
        await _context.Usuarios.FindAsync(id);

    public async Task<Usuario?> ObterPorEmailAsync(string email) =>
        await _context.Usuarios.FirstOrDefaultAsync(usuario => usuario.Email == email);

    public async Task<List<Usuario>> ObterTodosAsync() =>
        await _context.Usuarios.OrderBy(usuario => usuario.Nome).ToListAsync();

    public async Task AdicionarAsync(Usuario entidade) =>
        await _context.Usuarios.AddAsync(entidade);

    public void Atualizar(Usuario entidade) => _context.Usuarios.Update(entidade);

    public void Remover(Usuario entidade) => _context.Usuarios.Remove(entidade);
}
