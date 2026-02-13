using Gallerai.SharedKernel.Attributes;
using Gallerai.SharedKernel.Interfaces;

namespace Gallerai.SharedKernel.Settings;

[SettingsScope(SettingsScopes.Api)]
public class RedisSettings : ISettings
{
    public static string SectionName => "Redis";
    public string ConnectionString { get; set; } = string.Empty;
}
