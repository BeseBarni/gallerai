using Gallerai.SharedKernel.Attributes;
using Gallerai.SharedKernel.Interfaces;

namespace Gallerai.SharedKernel.Settings;

[SettingsScope(SettingsScopes.Shared)]
public class DatabaseSettings : ISettings
{
    public static string SectionName => "Database";
    public string ConnectionString { get; set; } = string.Empty;
}
