namespace Gallerai.Application.Interfaces;

public interface IImageService
{
    string GetImageUrlAsync(string key, string contentType, Dictionary<string, string>? metadata, CancellationToken ct = default);
}
