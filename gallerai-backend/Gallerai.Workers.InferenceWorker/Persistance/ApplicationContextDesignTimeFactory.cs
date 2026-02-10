using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Gallerai.Workers.InferenceWorker.Persistance;

public class WorkerDbContextDesignTimeFactory : IDesignTimeDbContextFactory<WorkerDbContext>
{
    public WorkerDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<WorkerDbContext> optionsBuilder = new();

        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=password");

        return new WorkerDbContext(optionsBuilder.Options);
    }
}
