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
    }

    public override async Task HandleAsync(GetImagePresignedURL.Request req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetImagePresignedURL.Command(req.Key, req.FileName, req.ContentType, req.FolderId), ct);

        await Send.OkAsync(result, cancellation: ct);
    }
}

public class ImagesUploadedEndpoint(IMediator mediator) : Endpoint<ImagesUploaded.Request, Result<ImagesUploaded.Response>>
{
    public override void Configure()
    {
        Post("/images/uploaded");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ImagesUploaded.Request req, CancellationToken ct)
    {
        var result = await mediator.Send(new ImagesUploaded.Command(req.Events ?? Array.Empty<ImagesUploaded.ImageUploadedEvent>()), ct);

        await Send.OkAsync(result, cancellation: ct);
    }
}

public class RemoveImageEndpoint(IMediator mediator) : Endpoint<RemoveImage.Request, Result>
{
    public override void Configure()
    {
        Delete("/images/{ImageId}");
    }

    public override async Task HandleAsync(RemoveImage.Request req, CancellationToken ct)
    {
        var result = await mediator.Send(new RemoveImage.Command(req.ImageId), ct);

        if (result.IsFailure)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}

