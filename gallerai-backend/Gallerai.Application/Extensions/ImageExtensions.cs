using Gallerai.Domain.Entities.ImageEntities;
using Gallerai.SharedKernel.Consts;

namespace Gallerai.Application.Extensions;

public static class ImageExtensions
{
    public static string GetImageStatusCacheKey(this Image image)
    {
        return CacheKeys.GetImageStatusCacheKey(image.ImageId, image.UserId);
    }
}
