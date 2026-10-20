using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PdvSistema.Infrastructure.Data;

public static class DatabaseMigrator
{
    public static async Task ApplyPendingMigrationsAsync(
        IServiceProvider services,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PdvSistemaDbContext>();

        try
        {
            logger.LogInformation("Applying pending database migrations.");
            await dbContext.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Database migration failed; API startup will be aborted.");
            throw;
        }
    }
}
