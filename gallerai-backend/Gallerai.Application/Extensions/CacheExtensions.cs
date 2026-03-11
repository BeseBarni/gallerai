using Gallerai.Application.Interfaces;
using Gallerai.SharedKernel.Consts;

namespace Gallerai.Application.Extensions;

internal static class CacheExtensions
{
    internal static async Task InvalidateImagesAndFolders(this ICacheService cache, string userId, params string[] imageIdList)
    {
        var folderListKey = CacheKeys.GetUserFoldersKey(userId);

        var tasks = new List<Task>
        {
            cache.RemoveAsync(folderListKey)
            //Open to extension: if we have more cache keys related to the image, we can add them here.
        };

        await Task.WhenAll(tasks);
    }
}
