using Gallerai.Application.Behaviors;
using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities;
using Gallerai.SharedKernel.Consts;
using Gallerai.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Application.Features.Folders;

public static class RenameFolder
{
    public record Request(Guid FolderId, string NewName);
    public record Command(Guid FolderId, string NewName) : IUserRequest
    {
        public string? UserId { get; set; }
    }
    public record Response(Guid FolderId, string Name);

    public sealed class Handler(IGalleraiDbContext context, ICacheService cacheService)
    {
        public async Task<Result<Response>> HandleAsync(Command request, CancellationToken ct)
        {
            var folder = await context.Folders
                .FirstOrDefaultAsync(f => f.FolderId == request.FolderId && f.UserId == request.UserId && f.DeletedAt == null, ct);

            if (folder is null)
            {
                return Error.NotFound(nameof(Folder), request.FolderId);
            }

            folder.Rename(request.NewName);
            await context.SaveChangesAsync(ct);

            await cacheService.RemoveAsync(CacheKeys.GetUserFoldersKey(request.UserId!));

            return new Response(folder.FolderId, folder.Name);
        }
    }
}
