using FastEndpoints;
using FluentValidation.Results;
using Gallerai.SharedKernel.Models;

namespace Gallerai.WebAPI.Extensions;

public static class EndpointExtensions
{
    public static async Task HandleResultAsync<TRequest, TResponse>(
            this Endpoint<TRequest, TResponse> endpoint,
            Result<TResponse> result,
            CancellationToken ct) where TRequest : notnull
    {
        if (result.IsSuccess)
        {
            if (result.Value is null)
            {
                await endpoint.HttpContext.Response.SendNoContentAsync(ct);
            }
            else
            {
                await endpoint.HttpContext.Response.SendAsync(result.Value, 200, cancellation: ct);
            }
            return;
        }

        await SendError(endpoint, result.Error, ct);
    }

    private static async Task SendError(BaseEndpoint endpoint, Error error, CancellationToken ct)
    {
        var statusCode = error.StatusCode == 0 ? 400 : error.StatusCode;
        await endpoint.HttpContext.Response.SendAsync(new { error }, statusCode, cancellation: ct);
    }

    public static async Task HandleNoContentResultAsync(
          this BaseEndpoint endpoint,
          Result? result,
          CancellationToken ct)
    {
        if (result is null)
        {
            await endpoint.HttpContext.Response.SendErrorsAsync([new ValidationFailure("error", "uknown error")], cancellation: ct);
            return;
        }

        if (result.IsSuccess)
        {
            await endpoint.HttpContext.Response.SendNoContentAsync(
                cancellation: ct);
            return;
        }

        await SendError(endpoint, result.Error, ct);
    }

    public static async Task HandleCreatedAsync<TRequest, TResponse>(
    this Endpoint<TRequest, TResponse> endpoint,
    Result<TResponse> result,
    string routeName,
    object routeValues,
    CancellationToken ct) where TRequest : notnull
    {
        if (result.IsSuccess)
        {
            await endpoint.HttpContext.Response.SendCreatedAtAsync(routeName, routeValues, result.Value, cancellation: ct);
            return;
        }

        await SendError(endpoint, result.Error, ct);
    }
}
