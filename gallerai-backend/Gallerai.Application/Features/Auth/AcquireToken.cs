using Gallerai.Application.Interfaces;
using Gallerai.SharedKernel.Consts;
using Gallerai.SharedKernel.Models;

namespace Gallerai.Application.Features.Auth;

public static class AcquireToken
{
    public record Request(string oneTimeCode);
    public record Response(string token);
    public record Command(string oneTimeCode);

    public class Handler(ICacheService cacheService)
    {
        public async Task<Result<Response>> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            var key = CacheKeys.GetTokenKey(request.oneTimeCode);

            var token = await cacheService.PopAsync<string>(key);

            if (token is null)
            {
                return Result<Response>.Failure(Error.NotFound(nameof(request.oneTimeCode), request.oneTimeCode));
            }

            return new Response(token);
        }
    }
}
