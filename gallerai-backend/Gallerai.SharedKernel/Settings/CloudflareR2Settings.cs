using Gallerai.SharedKernel.Interfaces;

namespace Gallerai.SharedKernel.Settings;

public sealed class CloudflareR2Settings : ISettings
{
    public static string SectionName => "CloudflareR2";
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string Bucket { get; set; } = string.Empty;
    public string PublicURL { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 1;
}
