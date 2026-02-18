using Gallerai.Application.Interfaces;
using Gallerai.SharedKernel.DTOs;
using Gallerai.SharedKernel.Models;
using Gallerai.SharedKernel.Settings;

namespace Gallerai.Application.Features.Auth;

public static class GoogleLogin
{
    public record Command();
    public sealed class Handler(IAuthService authService, GoogleAuthSettings googleAuthSettings)
    {
        private const string Provider = "Google";

        public Task<Result<ExternalAuthProperties>> HandleAsync(Command request, CancellationToken ct)
        {
            var properties = authService.GetExternalLoginProperties(Provider, googleAuthSettings.BackendCallbackUrl);

            return Task.FromResult(Result<ExternalAuthProperties>.Success(properties));
        }
    }
}
