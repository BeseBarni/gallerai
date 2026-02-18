using Gallerai.Domain.Entities.ImageEntities;
using Gallerai.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Gallerai.Infrastructure.Extensions;

internal static class DatabaseExtensions
{

    public static async Task<bool> TryAddEventAsync(this GalleraiDbContext context, ImageEvent imageEvent, CancellationToken ct)
    {
        try
        {
            await context.ImageEvents.AddAsync(imageEvent, ct);
            await context.SaveChangesAsync(ct);
            return true;
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyException(ex))
        {
            var entry = context.Entry(imageEvent);
            if (entry != null)
            {
                entry.State = EntityState.Detached;
            }
            return false;
        }
    }

    private static bool IsDuplicateKeyException(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505";
    }
}
