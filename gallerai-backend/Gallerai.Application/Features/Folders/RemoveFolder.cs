using Gallerai.Application.Behaviors;
using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities;
using Gallerai.SharedKernel.Consts;
using Gallerai.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Application.Features.Folders;

public static class RemoveFolder
{
    public record Request(Guid FolderId);
    public record Command(Guid FolderId) : IUserRequest
    {
        public string? UserId { get; set; }
    }

    public sealed class Handler(IGalleraiDbContext context, ICacheService cacheService)
    {
        public async Task<Result> HandleAsync(Command request, CancellationToken ct)
        {
            var folder = await context.Folders
                .Include(f => f.ImageList)
                .FirstOrDefaultAsync(f => f.FolderId == request.FolderId && f.UserId == request.UserId && f.DeletedAt == null, ct);

            if (folder is null)
            {
                return Result.Failure(Error.NotFound(nameof(Folder), request.FolderId));
            }

            folder.RemoveFolder();
            await context.SaveChangesAsync(ct);

            await cacheService.RemoveAsync(CacheKeys.GetUserFoldersKey(request.UserId!));

            return Result.Success();
        }
    }
}
