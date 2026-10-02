using Microsoft.EntityFrameworkCore;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Interfaces;
using PdvSistema.Infrastructure.Data;

namespace PdvSistema.Infrastructure.Repositories;

public class MovimentacaoEstoqueRepository : IMovimentacaoEstoqueRepository
{
    private readonly PdvSistemaDbContext _context;

    public MovimentacaoEstoqueRepository(PdvSistemaDbContext context) => _context = context;

    public Task<MovimentacaoEstoque?> ObterPorIdAsync(int id) =>
        _context.MovimentacoesEstoque
            .Include(movimentacao => movimentacao.Produto)
            .FirstOrDefaultAsync(movimentacao => movimentacao.Id == id);

    public Task<List<MovimentacaoEstoque>> ListarAsync(int? produtoId)
    {
        var query = _context.MovimentacoesEstoque
            .Include(movimentacao => movimentacao.Produto)
            .AsQueryable();

        if (produtoId.HasValue)
            query = query.Where(movimentacao => movimentacao.ProdutoId == produtoId.Value);

        return query.OrderByDescending(movimentacao => movimentacao.Data).ToListAsync();
    }

    public async Task AdicionarAsync(MovimentacaoEstoque movimentacao) =>
        await _context.MovimentacoesEstoque.AddAsync(movimentacao);
}