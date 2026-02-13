using Gallerai.Application.Interfaces;
using Gallerai.SharedKernel.Extensions;
using Gallerai.SharedKernel.Models;
using Gallerai.SharedKernel.Settings;
using MediatR;

namespace Gallerai.Application.Features.Auth;

public static class GoogleCallback
{
    public record Command() : IRequest<Result<GoogleCallbackResponse>>;

    public record GoogleCallbackResponse(string RedirectUrl);
    public sealed class Handler(IAuthService authService, ICacheService cacheService, IJwtTokenService jwtTokenService, JwtSettings jwtSetting, GoogleAuthSettings googleSettings) : IRequestHandler<Command, Result<GoogleCallbackResponse>>
    {
        public async Task<Result<GoogleCallbackResponse>> Handle(Command request, CancellationToken ct)
        {
            var loginResponse = await authService.HandleExternalLoginAsync();

            if (loginResponse is null)
            {
                return Result<GoogleCallbackResponse>.Failure(new Error("AUTH_FAILED", "External login failed or user creation failed."));
            }

            var (oneTimeCode, key) = loginResponse.GetTokenKey();

            var token = jwtTokenService.GenerateToken(loginResponse.UserId, loginResponse.Email, []);

            if (token is null)
            {
                return Result<GoogleCallbackResponse>.Failure(new Error("TOKEN_GENERATION_FAILED", "Failed to generate JWT token."));
            }

            await cacheService.SetAsync(key, token, jwtSetting.GetTokenOTPExpiry);

            return new GoogleCallbackResponse(googleSettings.GetRedirectUrl(oneTimeCode));
        }
    }
}
