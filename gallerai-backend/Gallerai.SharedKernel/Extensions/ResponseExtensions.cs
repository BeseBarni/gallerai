using Gallerai.SharedKernel.Consts;
using Gallerai.SharedKernel.DTOs;

namespace Gallerai.SharedKernel.Extensions;

public static class ResponseExtensions
{
    public static (string oneTimeCode, string key) GetTokenKey(this LoginResponse response)
    {
        var oneTimeCode = Guid.NewGuid().ToString("N");
        return (oneTimeCode, CacheKeys.GetTokenKey(oneTimeCode));
    }
}
