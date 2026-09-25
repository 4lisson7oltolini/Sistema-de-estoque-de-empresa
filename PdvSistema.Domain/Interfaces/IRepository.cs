namespace PdvSistema.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> ObterPorIdAsync(int id);
    Task<List<T>> ObterTodosAsync();
    Task AdicionarAsync(T entidade);
    void Atualizar(T entidade);
    void Remover(T entidade);
}