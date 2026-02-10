using Gallerai.Workers.InferenceWorker.Persistance;

namespace Gallerai.Workers.InferenceWorker.Extensions;

public static class AppExtensions
{
    public static async Task UseApplyMigrations(this IHost host)
    {
        var env = host.Services.GetRequiredService<IHostEnvironment>();
        if (!(env.IsDevelopment() || Environment.GetEnvironmentVariable("APPLY_MIGRATIONS") == "true")) return;

        Console.WriteLine("Waiting for database to be ready...");
        var dbReady = await host.Services.WaitForDatabaseAsync<WorkerDbContext>();

        if (dbReady)
        {
            await host.Services.ApplyMigrationsAsync<WorkerDbContext>();
        }
        else
        {
            Console.WriteLine("WARNING: Could not connect to database. Application may not function correctly.");
        }
    }
}
