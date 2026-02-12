using System.Security.Claims;
using Gallerai.Application.Interfaces;
using Gallerai.SharedKernel.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Gallerai.Infrastructure.Services;

internal sealed class AuthService(
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager) : IAuthService
{
    public ExternalAuthProperties GetExternalLoginProperties(string provider, string redirectUrl)
    {
        var result = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return new ExternalAuthProperties()
        {
            Items = result.Items,
            Provider = provider,
            RedirectUrl = redirectUrl
        };
    }

    public async Task<LoginResponse?> HandleExternalLoginAsync()
    {
        var info = await signInManager.GetExternalLoginInfoAsync();

        if (info is null) return null;

        var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

        if (user is null)
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email)) return null;

            user = await userManager.FindByEmailAsync(email);

            if (user is null)
            {
                user = new IdentityUser { UserName = email, Email = email };

                var createResult = await userManager.CreateAsync(user);

                if (!createResult.Succeeded) return null;
            }

            await userManager.AddLoginAsync(user, info);
        }

        return new LoginResponse(user.Id, user.Email!, user.UserName!);
    }
}
