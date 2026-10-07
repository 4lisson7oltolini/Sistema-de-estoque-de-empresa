using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using PdvSistema.Application.Services;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Enums;
using PdvSistema.Domain.Interfaces;

namespace PdvSistema.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task AutenticarAsync_DeveMigrarHashLegadoAposLoginValido()
    {
        const string senha = "SenhaLegadaSegura@2026";
        var senhaHashLegado = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(senha)));
        var usuario = new Usuario("Maria", "maria@teste.com", senhaHashLegado, PerfilUsuario.Vendedor);
        var repository = new FakeUsuarioRepository(usuario);
        var unitOfWork = new FakeUnitOfWork();
        var passwordHasher = new PasswordHasher<Usuario>();
        var service = new AuthService(repository, unitOfWork, passwordHasher);

        var response = await service.AutenticarAsync("MARIA@TESTE.COM", senha);

        Assert.NotNull(response);
        Assert.Equal("maria@teste.com", response.Email);
        Assert.DoesNotContain("SenhaHash", typeof(PdvSistema.Application.DTOs.Usuario.UsuarioResponse).GetProperties().Select(property => property.Name));
        Assert.NotEqual(senhaHashLegado, usuario.SenhaHash);
        Assert.Equal(PasswordVerificationResult.Success, passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha));
        Assert.Equal(1, unitOfWork.SaveCount);
    }

    [Fact]
    public async Task AutenticarAsync_DeveRejeitarSenhaIncorretaEUsuarioInativo()
    {
        var passwordHasher = new PasswordHasher<Usuario>();
        var usuario = new Usuario("Maria", "maria@teste.com", string.Empty, PerfilUsuario.Vendedor);
        usuario.SenhaHash = passwordHasher.HashPassword(usuario, "SenhaSegura@2026");
        var repository = new FakeUsuarioRepository(usuario);
        var service = new AuthService(repository, new FakeUnitOfWork(), passwordHasher);

        Assert.Null(await service.AutenticarAsync("maria@teste.com", "outra-senha-segura"));
        usuario.Desativar();
        Assert.Null(await service.AutenticarAsync("maria@teste.com", "SenhaSegura@2026"));
    }

    [Fact]
    public async Task CriarAdministradorInicialAsync_DevePermitirSomenteUmaConfiguracao()
    {
        var repository = new FakeUsuarioRepository();
        var service = new AuthService(repository, new FakeUnitOfWork(), new PasswordHasher<Usuario>());

        var admin = await service.CriarAdministradorInicialAsync("Admin", "ADMIN@TESTE.COM", "SenhaSegura@2026");

        Assert.Equal(PerfilUsuario.Administrador, admin.Perfil);
        Assert.Equal("admin@teste.com", admin.Email);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CriarAdministradorInicialAsync("Outro", "outro@teste.com", "OutraSenhaSegura@2026"));
    }

    private sealed class FakeUsuarioRepository(params Usuario[] usuarios) : IUsuarioRepository
    {
        public List<Usuario> Usuarios { get; } = [.. usuarios];

        public Task<Usuario?> ObterPorIdAsync(int id) => Task.FromResult(Usuarios.FirstOrDefault(usuario => usuario.Id == id));
        public Task<Usuario?> ObterPorEmailAsync(string email) => Task.FromResult(Usuarios.FirstOrDefault(usuario => string.Equals(usuario.Email, email, StringComparison.OrdinalIgnoreCase)));
        public Task<List<Usuario>> ObterTodosAsync() => Task.FromResult(Usuarios);
        public Task AdicionarAsync(Usuario entidade)
        {
            entidade.Id = Usuarios.Count + 1;
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