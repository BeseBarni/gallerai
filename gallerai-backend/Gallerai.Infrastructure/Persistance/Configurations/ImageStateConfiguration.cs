using Gallerai.Domain.Entities.ImageEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gallerai.Infrastructure.Persistance.Configurations;

internal sealed class ImageStateConfiguration : IEntityTypeConfiguration<ImageState>
{
    public void Configure(EntityTypeBuilder<ImageState> builder)
    {
        builder.HasKey(s => s.ImageId);

        builder.Property(s => s.Status)
            .IsRequired();

        builder
            .HasOne(s => s.Image)
            .WithOne(i => i.Status)
            .HasForeignKey<ImageState>(s => s.ImageId)
            .IsRequired();

        builder.HasIndex(s => s.Status);
    }
}
