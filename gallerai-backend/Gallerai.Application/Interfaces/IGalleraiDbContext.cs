using Gallerai.Domain.Entities.ImageEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Gallerai.Application.Interfaces;

public interface IGalleraiDbContext
{
    DbSet<Image> Images { get; }
    DbSet<ImageMetadata> ImageMetadata { get; }
    DbSet<ImageAnalysis> ImageAnalyses { get; }
    DbSet<ImageState> ImageStates { get; }
    DbSet<ImageEvent> ImageEvents { get; }
    DbSet<ImageTag> ImageTags { get; }

    DatabaseFacade Database { get; }

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    Task LockImagesAndStatuses(string[] keys, CancellationToken cancellationToken = default);
}
