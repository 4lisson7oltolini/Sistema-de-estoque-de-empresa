using PdvSistema.Domain.Entities;

namespace PdvSistema.Domain.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> ObterPorEmailAsync(string email);
}
