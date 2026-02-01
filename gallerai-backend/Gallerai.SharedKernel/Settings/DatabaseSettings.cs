using Gallerai.SharedKernel.Interfaces;

namespace Gallerai.SharedKernel.Settings;

public class DatabaseSettings : ISettings
{
    public static string SectionName => "Database";
    public string ConnectionString { get; set; } = string.Empty;
}
