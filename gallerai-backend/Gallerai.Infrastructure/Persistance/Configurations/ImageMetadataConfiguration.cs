using Gallerai.Domain.Entities.ImageEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gallerai.Infrastructure.Persistance.Configurations;

internal sealed class ImageMetadataConfiguration : IEntityTypeConfiguration<ImageMetadata>
{
    public void Configure(EntityTypeBuilder<ImageMetadata> builder)
    {
        builder.HasKey(m => m.ImageId);

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(m => m.Description)
            .HasMaxLength(2048);

        builder.OwnsOne(m => m.Camera, cb =>
        {
            cb.Property(c => c.Make).HasMaxLength(128);
            cb.Property(c => c.Model).HasMaxLength(128);
            cb.Property(c => c.LensModel).HasMaxLength(256);
            cb.Property(c => c.Software).HasMaxLength(128);
            cb.Property(c => c.CapturedAt);
        });

        builder.OwnsOne(m => m.Exposure, eb =>
        {
            eb.Property(e => e.Iso);
            eb.Property(e => e.Aperture);
            eb.Property(e => e.ShutterSpeedSeconds);
            eb.Property(e => e.FocalLengthMm);
            eb.Property(e => e.ExposureCompensation);
            eb.Property(e => e.Flash);
            eb.Property(e => e.WhiteBalance);
        });

        builder
            .HasOne(m => m.Image)
            .WithOne(i => i.Metadata)
            .HasForeignKey<ImageMetadata>(m => m.ImageId)
            .IsRequired();
    }
}
