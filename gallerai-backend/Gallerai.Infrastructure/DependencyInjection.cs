using Amazon.Runtime;
using Amazon.S3;
using Gallerai.Application.Interfaces;
using Gallerai.Infrastructure.Extensions;
using Gallerai.Infrastructure.Persistance;
using Gallerai.Infrastructure.Services;
using Gallerai.SharedKernel.Settings;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Gallerai.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGeneratedSettings(configuration);

        var dbConnection = configuration.GetConfiguration<DatabaseSettings>().ConnectionString;

        var cloudflareR2Settings = configuration.GetConfiguration<CloudflareR2Settings>();

        var rabbitMqSettings = configuration.GetConfiguration<RabbitMQSettings>();

        services.AddDbContext<GalleraiDbContext>(options =>
            options.UseNpgsql(dbConnection));

        services.AddScoped<IGalleraiDbContext>(provider => provider.GetRequiredService<GalleraiDbContext>());

        services.AddSingleton<IAmazonS3>(_ =>
        {
            var credentials = new BasicAWSCredentials(cloudflareR2Settings.AccessKeyId, cloudflareR2Settings.SecretAccessKey);

            var config = new AmazonS3Config
            {
                ServiceURL = cloudflareR2Settings.Endpoint,
                ForcePathStyle = true
            };

            return new AmazonS3Client(credentials, config);
        });

        services.AddScoped<IImageService, ImageService>();

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<GalleraiDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(1);
                o.UsePostgres();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(rabbitMqSettings.Host, "/", h =>
                {
                    h.Username(rabbitMqSettings.UserName);
                    h.Password(rabbitMqSettings.Password);
                });

                cfg.UseRawJsonSerializer();

                cfg.ConfigureEndpoints(ctx);
            });
        });


        return services;
    }
}
