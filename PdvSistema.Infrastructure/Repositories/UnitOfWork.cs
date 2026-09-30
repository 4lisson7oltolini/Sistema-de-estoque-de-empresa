using PdvSistema.Domain.Interfaces;
using PdvSistema.Infrastructure.Data;

namespace PdvSistema.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly PdvSistemaDbContext _context;

    public UnitOfWork(PdvSistemaDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}