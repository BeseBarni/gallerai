using Gallerai.Domain.Entities.ImageEntities;
using Gallerai.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Gallerai.Infrastructure.Extensions;

internal static class DatabaseExtensions
{
    public static async Task LockImagesAndStatuses(this GalleraiDbContext context, string[] keys, CancellationToken ct)
    {
        var keysParam = new NpgsqlParameter("keys", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text)
        {
            Value = keys
        };

        var lockSql = $@"
                    SELECT 1 FROM ""{nameof(context.Images)}"" AS ""i""
                    JOIN ""{nameof(context.ImageStates)}"" AS ""s"" 
                    ON ""i"".""{nameof(Image.ImageId)}"" = ""s"".""{nameof(ImageState.ImageId)}""
                    WHERE ""i"".""{nameof(Image.R2Key)}"" = ANY(@keys)
                    FOR UPDATE";

        await context.Database.ExecuteSqlRawAsync(lockSql, [keysParam], ct);
    }
}
