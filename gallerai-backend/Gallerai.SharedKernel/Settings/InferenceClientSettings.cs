using Gallerai.SharedKernel.Attributes;
using Gallerai.SharedKernel.Interfaces;

namespace Gallerai.SharedKernel.Settings;

[SettingsScope(SettingsScopes.Worker)]
public class InferenceClientSettings : ISettings
{
    public static string SectionName => "InferenceClient";
    public string Endpoint { get; set; } = string.Empty;
    public string ModelId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
