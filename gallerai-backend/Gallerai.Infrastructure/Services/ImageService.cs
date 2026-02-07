using Amazon.S3;
using Amazon.S3.Model;
using Gallerai.Application.Interfaces;
using Gallerai.SharedKernel.Settings;

namespace Gallerai.Infrastructure.Services;

internal sealed class ImageService(IAmazonS3 s3Client, CloudflareR2Settings cloudflareR2Settings) : IImageService
{
    public Task<string> GetImageUrlAsync(string key, string contentType, CancellationToken ct = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = cloudflareR2Settings.Bucket,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(cloudflareR2Settings.ExpiryMinutes),
            ContentType = contentType
        };

        var url = s3Client.GetPreSignedURL(request);

        return Task.FromResult(url);
    }
}
