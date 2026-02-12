namespace Gallerai.SharedKernel.DTOs;

public record LoginResponse(string UserId, string Email, string? UserName);

public class ExternalAuthProperties
{
    public string Provider { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
    public IDictionary<string, string?> Items { get; set; } = new Dictionary<string, string?>();
}
