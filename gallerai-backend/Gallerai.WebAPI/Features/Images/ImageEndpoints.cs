using FastEndpoints;
using Gallerai.Application.Features.Images;
using Gallerai.SharedKernel.Models;
using MediatR;

namespace Gallerai.WebAPI.Features.Images;

public class ImagePresignedUrl(IMediator mediator) : Endpoint<GetImagePresignedURL.Request, Result<GetImagePresignedURL.Response>>
{
    public override void Configure()
    {
        Post("/images/presigned-url");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetImagePresignedURL.Request req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetImagePresignedURL.Command(req.FileName, req.ContentType), ct);

        await Send.OkAsync(result, cancellation: ct);
    }
}

