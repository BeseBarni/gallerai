using Gallerai.SharedKernel.DTOs;

namespace Gallerai.Application.Interfaces;

public interface IAuthService
{
    ExternalAuthProperties GetExternalLoginProperties(string provider, string redirectUrl);

    Task<LoginResponse?> HandleExternalLoginAsync();
}
