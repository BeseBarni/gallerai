using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities.ImageEntities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Infrastructure.Persistance;

public class GalleraiDbContext : DbContext, IGalleraiDbContext
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GalleraiDbContext).Assembly);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
