using Gallerai.Domain.Entities.ImageEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gallerai.Infrastructure.Persistance.Configurations;

internal sealed class ImageAnalysisConfiguration : IEntityTypeConfiguration<ImageAnalysis>
{
    public void Configure(EntityTypeBuilder<ImageAnalysis> builder)
    {
        builder.HasKey(a => a.ImageId);

        builder.Property(a => a.AestheticScore)
            .IsRequired();

        builder.Property(a => a.Critique)
            .IsRequired()
            .HasMaxLength(4000);

        builder
            .HasOne(a => a.Image)
            .WithOne(i => i.Analysis)
            .HasForeignKey<ImageAnalysis>(a => a.ImageId)
            .IsRequired();
    }
}
