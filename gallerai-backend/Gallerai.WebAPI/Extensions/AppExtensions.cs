using Gallerai.Infrastructure.Extensions;
using Gallerai.Infrastructure.Persistance;

namespace Gallerai.WebAPI.Extensions;

public static class AppExtensions
{
    public static async Task UseApplyMigrations(this WebApplication app)
    {
        if (!(app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("APPLY_MIGRATIONS") == "true")) return;

        Console.WriteLine("Waiting for database to be ready...");
        var dbReady = await app.Services.WaitForDatabaseAsync<GalleraiDbContext>();

        if (dbReady)
        {
            await app.Services.ApplyMigrationsAsync<GalleraiDbContext>();
        }
        else
        {
            Console.WriteLine("WARNING: Could not connect to database. Application may not function correctly.");
        }
    }
}
