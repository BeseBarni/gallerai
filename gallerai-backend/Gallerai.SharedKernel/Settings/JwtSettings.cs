using System.ComponentModel.DataAnnotations;
using Gallerai.SharedKernel.Attributes;
using Gallerai.SharedKernel.Interfaces;

namespace Gallerai.SharedKernel.Settings;

[SettingsScope(SettingsScopes.Api)]
public class JwtSettings : ISettings
{
    public static string SectionName => "Jwt";

    [Required]
    public string Secret { get; set; } = null!;

    [Required]
    public string Issuer { get; set; } = null!;

    [Required]
    public string Audience { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int ExpiryMinutes { get; set; } = 60;

    public int TokenOTPExpirySeconds { get; set; } = 30;

    public TimeSpan GetTokenOTPExpiry => TimeSpan.FromSeconds(TokenOTPExpirySeconds);
}
