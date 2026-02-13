using Gallerai.Application.Interfaces;
using Gallerai.SharedKernel.DTOs;
using Gallerai.SharedKernel.Models;
using Gallerai.SharedKernel.Settings;
using MediatR;

namespace Gallerai.Application.Features.Auth;

public static class GoogleLogin
{
    public record Command() : IRequest<Result<ExternalAuthProperties>>;
    public sealed class Handler(IAuthService authService, GoogleAuthSettings googleAuthSettings) : IRequestHandler<Command, Result<ExternalAuthProperties>>
    {
        private const string Provider = "Google";

        public Task<Result<ExternalAuthProperties>> Handle(Command request, CancellationToken ct)
        {
            var properties = authService.GetExternalLoginProperties(Provider, googleAuthSettings.BackendCallbackUrl);

            return Task.FromResult(Result<ExternalAuthProperties>.Success(properties));
        }
    }
}
