using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Gallerai.Workers.InferenceWorker.Persistance;

public class WorkerDbContextDesignTimeFactory : IDesignTimeDbContextFactory<WorkerDbContext>
{
    public WorkerDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<WorkerDbContext> optionsBuilder = new();

        optionsBuilder.UseNpgsql("");

        return new WorkerDbContext(optionsBuilder.Options);
    }
}
