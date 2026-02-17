using Gallerai.SharedKernel.Activity;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
namespace Gallerai.Infrastructure.ServiceRegistration;

internal static class OpenTelemetry
{
    public static IServiceCollection AddGalleraiOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("Gallerai.API"))
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddHttpClientInstrumentation();
                metrics.AddNpgsqlInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddHttpClientInstrumentation();
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddNpgsql();
                tracing.AddRedisInstrumentation();
                tracing.AddSource(GalleraiActivity.GalleraiActivitySource.Name);
                tracing.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);
                tracing.AddSource("Microsoft.AspNetCore.SignalR.Server");
            })
            .UseOtlpExporter();
        return services;
    }
}
