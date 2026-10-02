using PdvSistema.Domain.Entities;

namespace PdvSistema.Domain.Interfaces;

public interface ICategoriaRepository : IRepository<Categoria>
{
    Task<bool> PossuiProdutosAsync(int categoriaId);
}