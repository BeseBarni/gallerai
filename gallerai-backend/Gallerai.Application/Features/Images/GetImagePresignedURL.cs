using System.Diagnostics;
using Gallerai.Application.Behaviors;
using Gallerai.Application.Extensions;
using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities.ImageEntities;
using Gallerai.SharedKernel.Enums;
using Gallerai.SharedKernel.Models;
using Gallerai.SharedKernel.Settings;

namespace Gallerai.Application.Features.Images;

public static class GetImagePresignedURL
{
    public record Request(Guid Key, string FileName, string ContentType, Guid FolderId);
    public record Command(Guid Key, string FileName, string ContentType, Guid FolderId) : IUserRequest
    {
        public string? UserId { get; set; }
    }
    public record Response(string UploadUrl, Guid ImageId, string Key, string CDNUrl, string Traceparent);

    public sealed class Handler(IImageService ImageService, IGalleraiDbContext Context, CloudflareR2Settings cloudflareR2Settings, ICacheService cacheService)
    {
        public async Task<Result<Response>> HandleAsync(Command request, CancellationToken ct)
        {
            var image = Image.Create(request.Key, request.UserId!, request.FolderId);

            var extension = Path.GetExtension(request.FileName);
            var storageKey = $"{image.ImageId}{extension}";

            image.SetStorageKey(storageKey);

            var activity = Activity.Current;

            var metadata = new Dictionary<string, string>();
            string? traceparent = string.Empty;
            if (activity?.Id is not null)
            {
                traceparent = activity.Id;
                metadata.Add("traceparent", traceparent);
            }

            var uploadUrl = ImageService.GetImageUrlAsync(
                        storageKey,
                        request.ContentType,
                        metadata,
                        ct
                    );

            if (uploadUrl is null)
            {
                return Result<Response>.Failure(new Error("IM_U_F", "Failed to generate presigned URL", 500));
            }
            await cacheService.SetAsync(image.GetImageStatusCacheKey(), ImageStatus.UPLOADING, TimeSpan.FromMinutes(5));

            Context.Images.Add(image);

            await Context.SaveChangesAsync(ct);


            return new Response(uploadUrl, image.ImageId, storageKey, string.Join('/', cloudflareR2Settings.PublicURL, storageKey), traceparent);
        }
    }
}
