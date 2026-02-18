using Gallerai.Application.Behaviors;
using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities;
using Gallerai.SharedKernel.Consts;
using Gallerai.SharedKernel.Models;

namespace Gallerai.Application.Features.Folders;

public static class AddFolder
{
    public record Request(string Name);
    public record Command(string Name) : IUserRequest
    {
        public string? UserId { get; set; }
    }
    public record Response(Guid FolderId, string Name);

    public sealed class Handler(IGalleraiDbContext context, ICacheService cacheService)
    {
        public async Task<Result<Response>> HandleAsync(Command request, CancellationToken ct)
        {
            var folder = Folder.Create(request.UserId!, request.Name);

            context.Folders.Add(folder);
            await context.SaveChangesAsync(ct);

            await cacheService.RemoveAsync(CacheKeys.GetUserFoldersKey(request.UserId!));

            return new Response(folder.FolderId, folder.Name);
        }
    }
}
