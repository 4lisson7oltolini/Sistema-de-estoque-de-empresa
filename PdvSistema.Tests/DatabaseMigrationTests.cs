using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PdvSistema.Domain.Entities;
using PdvSistema.Domain.Enums;
using PdvSistema.Infrastructure.Data;
using PdvSistema.Infrastructure.Repositories;

namespace PdvSistema.Tests;

public class DatabaseMigrationTests
{
    [Fact]
    public async Task ApplyPendingMigrationsAsync_CreatesAllMappedTables()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var services = CreateServices(connection);
        using var loggerFactory = LoggerFactory.Create(_ => { });

        await DatabaseMigrator.ApplyPendingMigrationsAsync(services, loggerFactory.CreateLogger("DatabaseMigrationTests"));

        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PdvSistemaDbContext>();
        var migrationNames = await dbContext.Database.GetAppliedMigrationsAsync();
        var tableNames = await GetTableNamesAsync(dbContext);

        Assert.Equal(["20260930170157_InitialCreate"], migrationNames);
        Assert.All(
            new[] { "Produtos", "Categorias", "Clientes", "Usuarios", "Vendas", "ItensVenda", "MovimentacoesEstoque" },
            tableName => Assert.Contains(tableName, tableNames));
    }

    [Fact]
    public async Task ApplyPendingMigrationsAsync_AllowsUsuarioRepositoryQueriesAfterMigration()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var services = CreateServices(connection);
        using var loggerFactory = LoggerFactory.Create(_ => { });

        await DatabaseMigrator.ApplyPendingMigrationsAsync(services, loggerFactory.CreateLogger("DatabaseMigrationTests"));

        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PdvSistemaDbContext>();
        var repository = new UsuarioRepository(dbContext);
        var usuario = new Usuario("Administradora", "admin@teste.com", "hash-de-teste", PerfilUsuario.Administrador);
        await repository.AdicionarAsync(usuario);
        await dbContext.SaveChangesAsync();

        var persistedUsuario = await repository.ObterPorEmailAsync("admin@teste.com");

        Assert.NotNull(persistedUsuario);
        Assert.Equal("Administradora", persistedUsuario.Nome);
    }

    [Fact]
    public async Task ApplyPendingMigrationsAsync_PropagatesConnectionFailure()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "database.db");
        await using var connection = new SqliteConnection($"Data Source={databasePath}");
        await using var services = CreateServices(connection);
        using var loggerFactory = LoggerFactory.Create(_ => { });

        await Assert.ThrowsAsync<SqliteException>(() =>
            DatabaseMigrator.ApplyPendingMigrationsAsync(services, loggerFactory.CreateLogger("DatabaseMigrationTests")));
    }

    private static ServiceProvider CreateServices(SqliteConnection connection)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<PdvSistemaDbContext>(options => options.UseSqlite(connection));
        return services.BuildServiceProvider();
    }

    private static async Task<HashSet<string>> GetTableNamesAsync(PdvSistemaDbContext dbContext)
    {
        var tableNames = new HashSet<string>(StringComparer.Ordinal);
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table'";
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            tableNames.Add(reader.GetString(0));

        return tableNames;
    }
}
