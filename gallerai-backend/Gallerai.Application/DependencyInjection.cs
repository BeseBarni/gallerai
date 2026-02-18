using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
namespace Gallerai.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
