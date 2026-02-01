using Gallerai.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Infrastructure.Persistance;

public class GalleraiDbContext : DbContext, IGalleraiDbContext
{
    public GalleraiDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GalleraiDbContext).Assembly);
    }
}
