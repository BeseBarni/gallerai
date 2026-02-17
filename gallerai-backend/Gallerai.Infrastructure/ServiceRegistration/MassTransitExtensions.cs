using Gallerai.Application.Features.Images.Consumers;
using Gallerai.Infrastructure.Extensions;
using Gallerai.Infrastructure.Persistance;
using Gallerai.SharedKernel.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gallerai.Infrastructure.ServiceRegistration;

public static class MassTransitExtensions
{
    public static IServiceCollection AddGalleraiMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSettings = configuration.GetConfiguration<RabbitMQSettings>();

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<GalleraiDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(1);
                o.UsePostgres();
                o.UseBusOutbox();
            });

            x.AddConsumer<AIInferenceFinishedEventConsumer>();
            x.AddConsumer<ImageUploadedEventConsumer>();

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
