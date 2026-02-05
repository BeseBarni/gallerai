using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities.ImageEntities;
using Gallerai.SharedKernel.Models;
using MediatR;

namespace Gallerai.Application.Features.Images;

public static class GetImagePresignedURL
{
    public record Request(string FileName, string ContentType);
    public record Command(string FileName, string ContentType) : IRequest<Result<Response>>;
    public record Response(string UploadUrl, Guid ImageId, string Key);

    public sealed class Handler(IImageService ImageService, IGalleraiDbContext Context) : IRequestHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            var image = Image.Create();

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

            return new Response(uploadUrl, image.ImageId, storageKey);
        }
    }
}
