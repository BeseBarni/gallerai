using Gallerai.Application.Behaviors;
using Gallerai.Application.Interfaces;
using Gallerai.Domain.Enums;
using Gallerai.SharedKernel.Models;
using Gallerai.SharedKernel.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gallerai.Application.Features.Folders;

public static class GetFolderImages
{
    public record Request(Guid FolderId);
    public record Command(Guid FolderId) : IRequest<Result<Response>>, IUserRequest
    {
        public string? UserId { get; set; }
    }

    public record ImageDto(
        Guid ImageId,
        Guid FolderId,
        string CdnUrl,
        ImageStatus Status,
        double? AestheticScore,
        string? Critique);

    public record Response(IReadOnlyCollection<ImageDto> Images);

    public sealed class Handler(
        IGalleraiDbContext context,
        IOptions<CloudflareR2Settings> r2Settings) : IRequestHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            var folderExists = await context.Folders
                .AnyAsync(f => f.FolderId == request.FolderId
                            && f.UserId == request.UserId
                            && f.DeletedAt == null, ct);

            if (!folderExists)
            {
                return Result<Response>.Failure(new Error("Folder.NotFound", "Folder not found."));
            }

            var publicUrl = r2Settings.Value.PublicURL;

            var images = await context.Images
                .Where(i => i.FolderId == request.FolderId
                         && i.UserId == request.UserId
                         && i.DeletedAt == null)
                .Select(i => new ImageDto(
                    i.ImageId,
                    i.FolderId,
                    i.R2Key != null ? $"{publicUrl.TrimEnd('/')}/{i.R2Key}" : string.Empty,
                    i.Status.Status,
                    i.Analysis != null ? i.Analysis.AestheticScore : null,
                    i.Analysis != null ? i.Analysis.Critique : null))
                .ToListAsync(ct);

            return new Response(images);
        }
    }
}
