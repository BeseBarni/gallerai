using FluentValidation;
using Gallerai.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
namespace Gallerai.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UserIdBehavior<,>));
        });
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
