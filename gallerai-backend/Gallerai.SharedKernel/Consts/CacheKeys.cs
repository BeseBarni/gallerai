namespace Gallerai.SharedKernel.Consts;

public static class CacheKeys
{
    public static string GetTokenKey(string oneTimeCode) => $"user-token:{oneTimeCode}";
}
