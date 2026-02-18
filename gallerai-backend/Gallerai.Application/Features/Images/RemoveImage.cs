using Gallerai.Application.Behaviors;
using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities.ImageEntities;
using Gallerai.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Application.Features.Images;

public static class RemoveImage
{
    public record Request(Guid ImageId);
    public record Command(Guid ImageId) : IUserRequest
    {
        public string? UserId { get; set; }
    }

    public sealed class Handler(IGalleraiDbContext context)
    {
        public async Task<Result> HandleAsync(Command request, CancellationToken ct)
        {
            var image = await context.Images
                .FirstOrDefaultAsync(i => i.ImageId == request.ImageId
                                       && i.UserId == request.UserId
                                       && i.DeletedAt == null, ct);

            if (image is null)
            {
                return Result.Failure(Error.NotFound(nameof(Image), request.ImageId));
            }

            image.DeletedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
