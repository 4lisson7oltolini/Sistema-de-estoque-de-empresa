using Microsoft.EntityFrameworkCore;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Interfaces;
using PdvSistema.Infrastructure.Data;

namespace PdvSistema.Infrastructure.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly AppDbContext _context;

        public ProdutoRepository(AppDbContext context) => _context = context;

        public async Task<Produto?> ObterPorIdAsync(int id) =>
            await _context.Produtos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Produto>> ListarTodosAsync() =>
            await _context.Produtos.Include(p => p.Categoria).ToListAsync();

        public async Task AdicionarAsync(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
        }
    }
}