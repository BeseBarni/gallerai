using Gallerai.Application.Behaviors;
using Gallerai.Application.Interfaces;
using Gallerai.SharedKernel.Consts;
using Gallerai.SharedKernel.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Application.Features.Folders;

public static class GetFolders
{
    public record Request();
    public record Command() : IRequest<Result<Response>>, IUserRequest
    {
        public string? UserId { get; set; }
    }
    public record FolderDto(Guid FolderId, string Name, int ImageCount);
    public record Response(IReadOnlyCollection<FolderDto> Folders);

    public sealed class Handler(IGalleraiDbContext context, ICacheService cacheService) : IRequestHandler<Command, Result<Response>>
    {
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            var cacheKey = CacheKeys.GetUserFoldersKey(request.UserId!);

            var folders = await cacheService.GetOrSetAsync(
                cacheKey,
                async () => await context.Folders
                    .Include(p => p.ImageList)
                    .Where(f => f.UserId == request.UserId && f.DeletedAt == null)
                    .Select(f => new FolderDto(f.FolderId, f.Name, f.ImageList.Count()))
                    .ToListAsync(ct),
                CacheExpiration);

            return new Response(folders ?? []);
        }
    }
}
