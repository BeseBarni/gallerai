using FastEndpoints;
using Gallerai.Application.Features.Auth;
using Gallerai.SharedKernel.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;

namespace Gallerai.WebAPI.Features.Auth;

public class GoogleLoginEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/google");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new GoogleLogin.Command(), ct);

        if (result.IsFailure)
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var authProperties = new AuthenticationProperties(result.Value.Items)
        {
            RedirectUri = result.Value.RedirectUrl
        };

        await Send.ResultAsync(Results.Challenge(authProperties, [result.Value.Provider]));
    }
}

public class GoogleCallbackEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/google/callback");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new GoogleCallback.Command(), ct);

        if (result.IsFailure || result is null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }
        var redirectUrl = result.Value.RedirectUrl;

        await Send.RedirectAsync(redirectUrl, allowRemoteRedirects: true);
    }
}

public class AcquireTokenEndpoint(IMediator mediator) : Endpoint<AcquireToken.Request, Result<AcquireToken.Response>>
{
    public override void Configure()
    {
        Post("/auth/acquire-token");
        AllowAnonymous();
    }
    public override async Task HandleAsync(AcquireToken.Request req, CancellationToken ct)
    {
        var result = await mediator.Send(new AcquireToken.Command(req.oneTimeCode), ct);

        if (result.IsFailure)
        {
            HttpContext.Response.StatusCode = 400; return;
        }

        await Send.OkAsync(result.Value, cancellation: ct);
    }
}
