namespace Gallerai.Application.Interfaces;

public interface IImageService
{
    Task<string> GetImageUrlAsync(string key, string contentType, Dictionary<string, string>? metadata, CancellationToken ct = default);
}
