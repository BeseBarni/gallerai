using Gallerai.Application.Interfaces;
using Gallerai.SharedKernel.Models;
using MediatR;

namespace Gallerai.Application.Behaviors;

public sealed class UserIdBehavior<TRequest, TResponse>(ICurrentUserService currentUserService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly Error UserNotAuthenticatedError = new("AUTH_001", "User not authenticated");

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IUserRequest userRequest)
        {
            userRequest.UserId ??= currentUserService.UserId;

            if (string.IsNullOrEmpty(userRequest.UserId))
            {
                return CreateFailureResult();
            }
        }

        return await next();
    }

    private static TResponse CreateFailureResult()
    {
        var responseType = typeof(TResponse);

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var innerType = responseType.GetGenericArguments()[0];
            var failureMethod = typeof(Result<>).MakeGenericType(innerType).GetMethod("Failure", [typeof(Error)]);
            return (TResponse)failureMethod!.Invoke(null, [UserNotAuthenticatedError])!;
        }

        throw new InvalidOperationException("IUserRequest must return a Result<T> type.");
    }
}
