using Microsoft.EntityFrameworkCore;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Interfaces;
using PdvSistema.Infrastructure.Data;

namespace PdvSistema.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly PdvSistemaDbContext _context;

    public ProdutoRepository(PdvSistemaDbContext context) => _context = context;

    public async Task<Produto?> ObterPorIdAsync(int id) =>
        await _context.Produtos.FindAsync(id);

    public async Task<Produto?> ObterPorIdComCategoriaAsync(int id) =>
        await _context.Produtos.Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Produto>> ObterTodosAsync() =>
        await _context.Produtos.ToListAsync();

    public async Task<List<Produto>> ObterPorCategoriaAsync(int categoriaId) =>
        await _context.Produtos.Where(p => EF.Property<int>(p, "CategoriaId") == categoriaId)
            .ToListAsync();

    public async Task AdicionarAsync(Produto entidade) =>
        await _context.Produtos.AddAsync(entidade);

    public void Atualizar(Produto entidade) => _context.Produtos.Update(entidade);

    public void Remover(Produto entidade) => _context.Produtos.Remove(entidade);
}