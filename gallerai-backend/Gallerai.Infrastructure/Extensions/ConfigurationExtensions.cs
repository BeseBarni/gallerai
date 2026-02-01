using Gallerai.SharedKernel.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Gallerai.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    public static T GetConfiguration<T>(this IConfiguration configuration) where T : ISettings, new()
    {
        return configuration.GetSection(T.SectionName).Get<T>()
            ?? throw new InvalidOperationException($"{T.SectionName} settings are not configured");
    }
}
