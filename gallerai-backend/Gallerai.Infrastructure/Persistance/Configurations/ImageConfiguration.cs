using Gallerai.Domain.Entities.ImageEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gallerai.Infrastructure.Persistance.Configurations;

internal sealed class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasKey(i => i.ImageId);

        builder.Property(i => i.R2Key)
            .HasMaxLength(256);

        builder.Property(i => i.Size);

        builder
            .HasOne(i => i.Status)
            .WithOne(s => s.Image)
            .HasForeignKey<ImageState>(s => s.ImageId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(i => i.Metadata)
            .WithOne(m => m.Image)
            .HasForeignKey<ImageMetadata>(m => m.ImageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(i => i.Analysis)
            .WithOne(a => a.Image)
            .HasForeignKey<ImageAnalysis>(a => a.ImageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(i => i.ImageEvents)
            .WithOne(e => e.Image)
            .HasForeignKey(e => e.ImageId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(i => i.ImageTags)
            .WithMany(t => t.ImageList);

        builder.HasIndex(i => i.R2Key).IsUnique();
        builder.HasIndex(i => i.UploadedAt);
    }
}
