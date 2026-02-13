using Gallerai.Application.Interfaces;
using Gallerai.SharedKernel.Consts;
using Gallerai.SharedKernel.Models;
using MediatR;

namespace Gallerai.Application.Features.Auth;

public static class AcquireToken
{
    public record Request(string oneTimeCode);
    public record Response(string token);
    public record Command(string oneTimeCode) : IRequest<Result<Response>>;

    public class Handler(ICacheService cacheService) : IRequestHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var key = CacheKeys.GetTokenKey(request.oneTimeCode);

            var token = await cacheService.PopAsync<string>(key);

            if (token is null)
            {
                return Result<Response>.Failure(new Error("KEY_NOT_EXISTS", "The key is not found"));
            }

            return new Response(token);
        }
    }
}
