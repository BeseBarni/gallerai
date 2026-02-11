using Gallerai.Domain.Entities.ImageEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gallerai.Infrastructure.Persistance.Configurations;

public class ImageEventConfiguration : IEntityTypeConfiguration<ImageEvent>
{
    public void Configure(EntityTypeBuilder<ImageEvent> builder)
    {
        builder.HasKey(e => e.ImageEventId);

        builder.Property(e => e.LastUpdate)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.Message)
            .HasMaxLength(1024);

        builder
            .HasOne(e => e.Image)
            .WithMany(i => i.ImageEvents)
            .HasForeignKey(e => e.ImageId)
            .IsRequired();

        builder.HasIndex(e => new { e.ImageId, e.LastUpdate });

        builder.HasIndex(e => new { e.ImageId, e.Status })
            .IsUnique()
            .HasDatabaseName("IX_ImageEvents_ImageId_Status_Unique");
    }
}
