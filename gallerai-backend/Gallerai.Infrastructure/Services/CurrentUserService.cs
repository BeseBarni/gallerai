using System.Security.Claims;
using Gallerai.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Gallerai.Infrastructure.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?
        .User.FindFirstValue(ClaimTypes.NameIdentifier);
}
