using Gallerai.Application.Interfaces;
using Gallerai.Infrastructure.Extensions;
using Gallerai.Infrastructure.ServiceRegistration;
using Gallerai.Infrastructure.Services;
using Gallerai.SharedKernel.Settings;
using Gallerai.SignalR.Shared.Consts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
namespace Gallerai.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGeneratedSettings(configuration);

        services.AddGalleraiPersistance(configuration);

        services.AddGalleraiAuthentication(configuration);

        services.AddGalleraiMassTransit(configuration);

        services.AddCloudflareR2(configuration);

        services.AddGalleraiOpenTelemetry();

        var redisSettings = configuration.GetConfiguration<RedisSettings>();

        services.AddSignalR()
            .AddStackExchangeRedis(redisSettings.ConnectionString, options =>
            {
                options.Configuration.ChannelPrefix = RedisChannel.Literal(ChannelConsts.Gallerai);
            });

        services.AddSingleton<IConnectionMultiplexer>(config =>
        {
            return ConnectionMultiplexer.Connect(redisSettings.ConnectionString);
        });

        services.AddSingleton<ICacheService, RedisCacheService>();

        services.AddScoped<INotificationService, SignalRNotificationService>();

        return services;
    }
}
