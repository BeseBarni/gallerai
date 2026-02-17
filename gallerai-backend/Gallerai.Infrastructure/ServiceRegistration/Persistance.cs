using Gallerai.Application.Interfaces;
using Gallerai.Infrastructure.Extensions;
using Gallerai.Infrastructure.Persistance;
using Gallerai.SharedKernel.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gallerai.Infrastructure.ServiceRegistration;

internal static class Persistance
{
    public static IServiceCollection AddGalleraiPersistance(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConnection = configuration.GetConfiguration<DatabaseSettings>().ConnectionString;

        services.AddDbContext<GalleraiDbContext>(options =>
            options.UseNpgsql(dbConnection));

        services.AddScoped<IGalleraiDbContext>(provider => provider.GetRequiredService<GalleraiDbContext>());

        return services;
    }
}
