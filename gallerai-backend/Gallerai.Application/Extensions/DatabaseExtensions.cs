using Gallerai.Application.Interfaces;
using Gallerai.Domain.Entities.ImageEntities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Gallerai.Application.Extensions;

internal static class DatabaseExtensions
{
    public async static Task LockImagesAndStatuses(this IGalleraiDbContext context, string[] keys, CancellationToken ct)
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
