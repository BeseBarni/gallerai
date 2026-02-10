using Gallerai.SharedKernel.Interfaces;

namespace Gallerai.Workers.InferenceWorker.Extensions;

public static class ConfigurationExtensions
{
    public static T GetConfiguration<T>(this IConfiguration configuration) where T : ISettings, new()
    {
        return configuration.GetSection(T.SectionName).Get<T>()
            ?? throw new InvalidOperationException($"{T.SectionName} settings are not configured");
    }
}
