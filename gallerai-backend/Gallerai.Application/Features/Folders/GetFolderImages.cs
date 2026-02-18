using Gallerai.Application.Behaviors;
using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities;
using Gallerai.SharedKernel.Enums;
using Gallerai.SharedKernel.Models;
using Gallerai.SharedKernel.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gallerai.Application.Features.Folders;

public static class GetFolderImages
{
    public record Request(Guid FolderId);
    public record Command(Guid FolderId) : IUserRequest
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
        IOptions<CloudflareR2Settings> r2Settings)
    {
        public async Task<Result<Response>> HandleAsync(Command request, CancellationToken ct)
        {
            var folderExists = await context.Folders
                .AnyAsync(f => f.FolderId == request.FolderId
                            && f.UserId == request.UserId
                            && f.DeletedAt == null, ct);

            if (!folderExists)
            {
                return Result<Response>.Failure(Error.NotFound(nameof(Folder), request.FolderId));
            }

            var publicUrl = r2Settings.Value.PublicURL;

            var images = await context.Images
                .AsNoTracking()
                .Where(i => i.FolderId == request.FolderId
                         && i.UserId == request.UserId
                         && i.DeletedAt == null)
                .Select(i => new ImageDto(
                    i.ImageId,
                    i.FolderId,
                    i.R2Key != null ? $"{publicUrl.TrimEnd('/')}/{i.R2Key}" : string.Empty,
                    i.ImageEvents
                        .OrderByDescending(ie => ie.Status)
                        .Select(ie => ie.Status)
                        .First(),
                    i.Analysis != null ? i.Analysis.AestheticScore : null,
                    i.Analysis != null ? i.Analysis.Critique : null))
                .ToListAsync(ct);

            return new Response(images);
        }
    }
}
