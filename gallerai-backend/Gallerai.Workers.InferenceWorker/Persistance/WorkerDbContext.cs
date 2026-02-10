using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Workers.InferenceWorker.Persistance;

public sealed class WorkerDbContext : DbContext
{
    public WorkerDbContext(DbContextOptions options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
