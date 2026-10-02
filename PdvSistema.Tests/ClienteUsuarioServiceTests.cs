using PdvSistema.Application.DTOs.Cliente;
using PdvSistema.Application.DTOs.Usuario;
using PdvSistema.Application.Services;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Enums;
using PdvSistema.Domain.Interfaces;

namespace PdvSistema.Tests;

public class ClienteUsuarioServiceTests
{
    [Fact]
    public async Task ClienteService_CriarAsync_DeveNormalizarDados()
    {
        var repository = new FakeClienteRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new ClienteService(repository, unitOfWork);

        var response = await service.CriarAsync(new CriarClienteRequest
        {
            Nome = "  João da Silva  ",
            Documento = " 12345678900 ",
            Telefone = " 11999999999 ",
            Email = " joao@email.com "
        });

        Assert.Equal("João da Silva", response.Nome);
        Assert.Equal("12345678900", response.Documento);
        Assert.Equal("11999999999", response.Telefone);
        Assert.Equal("joao@email.com", response.Email);
        Assert.Equal(1, unitOfWork.SaveCount);
        Assert.Single(repository.Clientes);
    }

    [Fact]
    public async Task UsuarioService_CriarAsync_DeveCriptografarSenhaEValidarDados()
    {
        var repository = new FakeUsuarioRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new UsuarioService(repository, unitOfWork);

        var response = await service.CriarAsync(new CriarUsuarioRequest
        {
            Nome = "Maria",
            Email = "maria@teste.com",
            Senha = "123456",
            Perfil = PerfilUsuario.Administrador
        });

        Assert.Equal("Maria", response.Nome);
        Assert.Equal("maria@teste.com", response.Email);
        Assert.NotEqual("123456", response.SenhaHash);
        Assert.Equal(PerfilUsuario.Administrador, response.Perfil);
        Assert.Equal(1, unitOfWork.SaveCount);
        Assert.Single(repository.Usuarios);
    }

    private sealed class FakeClienteRepository : IClienteRepository
    {
        public List<Cliente> Clientes { get; } = [];

        public Task<Cliente?> ObterPorIdAsync(int id) => Task.FromResult(Clientes.FirstOrDefault(c => c.Id == id));
        public Task<List<Cliente>> ObterTodosAsync() => Task.FromResult(Clientes);
        public Task AdicionarAsync(Cliente entidade)
        {
            entidade.Id = Clientes.Count + 1;
            Clientes.Add(entidade);
            return Task.CompletedTask;
        }
        public void Atualizar(Cliente entidade) { }
        public void Remover(Cliente entidade) => Clientes.Remove(entidade);
    }

    private sealed class FakeUsuarioRepository : IUsuarioRepository
    {
        public List<Usuario> Usuarios { get; } = [];

        public Task<Usuario?> ObterPorIdAsync(int id) => Task.FromResult(Usuarios.FirstOrDefault(u => u.Id == id));
        public Task<Usuario?> ObterPorEmailAsync(string email) => Task.FromResult(Usuarios.FirstOrDefault(u => u.Email == email));
        public Task<List<Usuario>> ObterTodosAsync() => Task.FromResult(Usuarios);
        public Task AdicionarAsync(Usuario entidade)
        {
            Usuarios.Add(entidade);
            return Task.CompletedTask;
        }
        public void Atualizar(Usuario entidade) { }
        public void Remover(Usuario entidade) => Usuarios.Remove(entidade);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveCount++;
            return Task.FromResult(1);
        }
    }
}
