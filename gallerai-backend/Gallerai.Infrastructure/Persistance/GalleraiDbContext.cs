using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities.ImageEntities;
using Gallerai.Infrastructure.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Infrastructure.Persistance;

public class GalleraiDbContext : IdentityDbContext, IGalleraiDbContext
{
    public GalleraiDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Image> Images { get; set; } = null!;
    public DbSet<ImageMetadata> ImageMetadata { get; set; } = null!;
    public DbSet<ImageAnalysis> ImageAnalyses { get; set; } = null!;
    public DbSet<ImageState> ImageStates { get; set; } = null!;
    public DbSet<ImageEvent> ImageEvents { get; set; } = null!;
    public DbSet<ImageTag> ImageTags { get; set; } = null!;

    public Task LockImagesAndStatuses(string[] keys, CancellationToken cancellationToken = default)
    {
        return DatabaseExtensions.LockImagesAndStatuses(this, keys, cancellationToken);
    }

    public Task<bool> TryAddEventAsync(ImageEvent imageEvent, CancellationToken ct = default)
    {
        return DatabaseExtensions.TryAddEventAsync(this, imageEvent, ct);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GalleraiDbContext).Assembly);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
