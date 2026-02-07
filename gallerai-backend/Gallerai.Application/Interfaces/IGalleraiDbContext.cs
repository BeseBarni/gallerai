using Gallerai.Domain.Entities.ImageEntities;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Application.Interfaces;

public interface IGalleraiDbContext
{
    DbSet<Image> Images { get; }
    DbSet<ImageMetadata> ImageMetadata { get; }
    DbSet<ImageAnalysis> ImageAnalyses { get; }
    DbSet<ImageState> ImageStates { get; }
    DbSet<ImageEvent> ImageEvents { get; }
    DbSet<ImageTag> ImageTags { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
