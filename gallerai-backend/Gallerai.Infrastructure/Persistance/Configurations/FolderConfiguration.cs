using Gallerai.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gallerai.Infrastructure.Persistance.Configurations;

internal sealed class FolderConfiguration : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.HasKey(f => f.FolderId);

        builder
            .HasMany(f => f.ImageList)
            .WithOne(i => i.Folder)
            .HasForeignKey(i => i.FolderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.Name).IsRequired().HasMaxLength(255);

        builder.HasIndex(f => f.UserId);
    }
}
