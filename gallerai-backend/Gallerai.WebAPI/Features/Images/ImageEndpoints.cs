using FastEndpoints;
using Gallerai.Application.Features.Images;
using Gallerai.SharedKernel.Models;
using Wolverine;

namespace Gallerai.WebAPI.Features.Images;

public class ImagePresignedUrl(IMessageBus bus) : Endpoint<GetImagePresignedURL.Request, Result<GetImagePresignedURL.Response>>
{
    public override void Configure()
    {
        Post("/images/presigned-url");
    }

    public override async Task HandleAsync(GetImagePresignedURL.Request req, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<GetImagePresignedURL.Response>>(new GetImagePresignedURL.Command(req.Key, req.FileName, req.ContentType, req.FolderId), ct);

        await Send.OkAsync(result, cancellation: ct);
    }
}

public class ImagesUploadedEndpoint(IMessageBus bus) : Endpoint<ImagesUploaded.Request, Result<ImagesUploaded.Response>>
{
    public override void Configure()
    {
        Post("/images/uploaded");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ImagesUploaded.Request req, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<ImagesUploaded.Response>>(new ImagesUploaded.Command(req.Events ?? Array.Empty<ImagesUploaded.ImageUploadedR2>()), ct);

        await Send.OkAsync(result, cancellation: ct);
    }
}

public class RemoveImageEndpoint(IMessageBus bus) : Endpoint<RemoveImage.Request, Result>
{
    public override void Configure()
    {
        Delete("/images/{ImageId}");
    }

    public override async Task HandleAsync(RemoveImage.Request req, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(new RemoveImage.Command(req.ImageId), ct);

        if (result.IsFailure)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}

