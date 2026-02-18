using Gallerai.Application.Interfaces;

namespace Gallerai.Application.Behaviors;

public static class UserIdMiddleware
{
    public static void Before(IUserRequest message, ICurrentUserService currentUserService)
    {
        message.UserId ??= currentUserService.UserId;

        if (string.IsNullOrEmpty(message.UserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
    }
}
