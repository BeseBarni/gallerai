namespace Gallerai.Application.Interfaces;

public interface IImageService
{
    Task<string> GetImageUrlAsync(string key, string contentType, CancellationToken ct = default);
}
