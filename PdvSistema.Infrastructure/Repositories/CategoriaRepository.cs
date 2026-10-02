using Microsoft.EntityFrameworkCore;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Interfaces;
using PdvSistema.Infrastructure.Data;

namespace PdvSistema.Infrastructure.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly PdvSistemaDbContext _context;

    public CategoriaRepository(PdvSistemaDbContext context) => _context = context;

    public Task<Categoria?> ObterPorIdAsync(int id) =>
        _context.Categorias.FindAsync(id).AsTask();

    public Task<List<Categoria>> ObterTodosAsync() =>
        _context.Categorias.OrderBy(categoria => categoria.Nome).ToListAsync();

    public Task<bool> PossuiProdutosAsync(int categoriaId) =>
        _context.Produtos.AnyAsync(produto => produto.CategoriaId == categoriaId);

    public async Task AdicionarAsync(Categoria entidade) =>
        await _context.Categorias.AddAsync(entidade);

    public void Atualizar(Categoria entidade) => _context.Categorias.Update(entidade);

    public void Remover(Categoria entidade) => _context.Categorias.Remove(entidade);
}