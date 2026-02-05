using Gallerai.Domain.Entities.ImageEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gallerai.Infrastructure.Persistance.Configurations;

internal sealed class ImageTagConfiguration : IEntityTypeConfiguration<ImageTag>
{
    public void Configure(EntityTypeBuilder<ImageTag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Tag)
            .IsRequired()
            .HasMaxLength(128);

        builder
            .HasMany(t => t.ImageList)
            .WithMany(i => i.ImageTags);

        builder.HasIndex(t => t.Tag).IsUnique();
    }
}
