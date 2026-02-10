using Microsoft.EntityFrameworkCore;

namespace Gallerai.Workers.InferenceWorker.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync<T>(this IServiceProvider serviceProvider) where T : DbContext
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();

        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                Console.WriteLine("Applying pending migrations...");
                foreach (var migration in pendingMigrations)
                {
                    Console.WriteLine($"  - {migration}");
                }
                await context.Database.MigrateAsync();
                Console.WriteLine("Migrations applied successfully.");
            }
            else
            {
                Console.WriteLine("Database is up to date.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying migrations: {ex.Message}");
            throw;
        }
    }

    public static async Task<bool> WaitForDatabaseAsync<T>(this IServiceProvider serviceProvider, int maxRetries = 30, int delaySeconds = 2) where T : DbContext
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                Console.WriteLine($"Attempting to connect to database (attempt {i + 1}/{maxRetries})...");
                await context.Database.CanConnectAsync();
                Console.WriteLine("Database connection successful!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
                if (i < maxRetries - 1)
                {
                    Console.WriteLine($"Retrying in {delaySeconds} seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }
        }

        Console.WriteLine("Failed to connect to database after maximum retries.");
        return false;
    }
}
