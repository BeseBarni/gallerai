using Gallerai.SharedKernel.Interfaces;

namespace Gallerai.SharedKernel.Settings;

public class RabbitMQSettings : ISettings
{
    public static string SectionName => "RabbitMQ";
    public string Host { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
