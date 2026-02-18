using Gallerai.Application.Behaviors;
using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities;
using Gallerai.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Application.Features.Folders;

public static class GetFolderById
{
    public record Request(Guid FolderId);
    public record Command(Guid FolderId) : IUserRequest
    {
        public string? UserId { get; set; }
    }
    public record Response(Guid FolderId, string Name, int ImageCount);

    public sealed class Handler(IGalleraiDbContext context)
    {
        public async Task<Result<Response>> HandleAsync(Command request, CancellationToken ct)
        {
            var folder = await context.Folders
                .Include(f => f.ImageList)
                .FirstOrDefaultAsync(f => f.FolderId == request.FolderId && f.UserId == request.UserId && f.DeletedAt == null, ct);

            if (folder is null)
            {
                return Error.NotFound(nameof(Folder), request.FolderId);
            }

            return new Response(folder.FolderId, folder.Name, folder.ImageList.Count);
        }
    }
}
