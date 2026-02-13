using Gallerai.Application.Behaviors;
using Gallerai.Application.Extensions;
using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities.ImageEntities;
using Gallerai.Domain.Enums;
using Gallerai.SharedKernel.Models;
using Gallerai.SharedKernel.Settings;
using MediatR;

namespace Gallerai.Application.Features.Images;

public static class GetImagePresignedURL
{
    public record Request(Guid Key, string FileName, string ContentType, Guid FolderId);
    public record Command(Guid Key, string FileName, string ContentType, Guid FolderId) : IRequest<Result<Response>>, IUserRequest
    {
        public string? UserId { get; set; }
    }
    public record Response(string UploadUrl, Guid ImageId, string Key, string CDNUrl);

    public sealed class Handler(IImageService ImageService, IGalleraiDbContext Context, CloudflareR2Settings cloudflareR2Settings, ICacheService cacheService) : IRequestHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            var image = Image.Create(request.Key, request.UserId!, request.FolderId);

            var extension = Path.GetExtension(request.FileName);
            var storageKey = $"{image.ImageId}{extension}";

            image.SetStorageKey(storageKey);


            var uploadUrl = await ImageService.GetImageUrlAsync(
                        storageKey,
                        request.ContentType,
                        ct
                    );

            if (uploadUrl is null)
            {
                return Result<Response>.Failure(new Error("IM_U_F", "Failed to generate presigned URL"));
            }

            Context.Images.Add(image);

            await Context.SaveChangesAsync(ct);

            await cacheService.SetAsync(image.GetImageStatusCacheKey(), ImageStatus.UPLOADING, TimeSpan.FromMinutes(5));

            return new Response(uploadUrl, image.ImageId, storageKey, string.Join('/', cloudflareR2Settings.PublicURL, storageKey));
        }
    }
}
