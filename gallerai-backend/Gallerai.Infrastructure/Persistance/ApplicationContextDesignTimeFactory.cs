using Gallerai.SharedKernel.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Gallerai.Infrastructure.Persistance;

public class GalleraiDbContextDesignTimeFactory : IDesignTimeDbContextFactory<GalleraiDbContext>
{
    public GalleraiDbContext CreateDbContext(string[] args)
    {
        var currentDir = Directory.GetCurrentDirectory();
        var basePath = Path.Combine(currentDir, "..", "Gallerai.WebAPI");

        if (!Directory.Exists(basePath))
        {
            basePath = Path.Combine(currentDir, "Gallerai.WebAPI");
        }

        if (!Directory.Exists(basePath))
        {
            basePath = currentDir;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var databaseSettings = new DatabaseSettings();
        configuration.GetSection(DatabaseSettings.SectionName).Bind(databaseSettings);

        DbContextOptionsBuilder<GalleraiDbContext> optionsBuilder = new();

        string? connectionString = databaseSettings.ConnectionString;

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                $"Could not find connection string in Database:ConnectionString. " +
                $"Current directory: {currentDir}, Config base path: {basePath}");
        }

        optionsBuilder.UseNpgsql(connectionString);

        return new GalleraiDbContext(optionsBuilder.Options);
    }
}
