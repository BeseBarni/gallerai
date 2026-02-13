using Gallerai.SharedKernel.Attributes;
using Gallerai.SharedKernel.Interfaces;

namespace Gallerai.SharedKernel.Settings;

[SettingsScope(SettingsScopes.Api)]
public class GoogleAuthSettings : ISettings
{
    public static string SectionName => "GoogleAuth";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string FrontendRedirectUrl { get; set; } = string.Empty;
    public string BackendCallbackUrl { get; set; } = string.Empty;
    public string GetRedirectUrl(string oneTimeCode) { return $"{FrontendRedirectUrl}?oneTimeCode={oneTimeCode}"; }
}
