using PdvSistema.Domain.Entities;
namespace PdvSistema.Domain.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<Produto?> ObterPorIdComCategoriaAsync(int id);
    Task<List<Produto>> ObterPorCategoriaAsync(int categoriaId);
}