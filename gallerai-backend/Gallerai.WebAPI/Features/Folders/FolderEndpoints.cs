using FastEndpoints;
using Gallerai.Application.Features.Folders;
using Gallerai.SharedKernel.Models;
using MediatR;

namespace Gallerai.WebAPI.Features.Folders;

public class GetFoldersEndpoint(IMediator mediator) : EndpointWithoutRequest<Result<GetFolders.Response>>
{
    public override void Configure()
    {
        Get("/folders");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await mediator.Send(new GetFolders.Command(), ct);

        await Send.OkAsync(result, cancellation: ct);
    }
}

public class AddFolderEndpoint(IMediator mediator) : Endpoint<AddFolder.Request, Result<AddFolder.Response>>
{
    public override void Configure()
    {
        Post("/folders");
    }

    public override async Task HandleAsync(AddFolder.Request req, CancellationToken ct)
    {
        var result = await mediator.Send(new AddFolder.Command(req.Name), ct);

        await Send.CreatedAtAsync<GetFoldersEndpoint>(null, result, cancellation: ct);
    }
}

public class RenameFolderEndpoint(IMediator mediator) : Endpoint<RenameFolder.Request, Result<RenameFolder.Response>>
{
    public override void Configure()
    {
        Put("/folders/{FolderId}");
    }

    public override async Task HandleAsync(RenameFolder.Request req, CancellationToken ct)
    {
        var result = await mediator.Send(new RenameFolder.Command(req.FolderId, req.NewName), ct);

        if (result.IsFailure)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(result, cancellation: ct);
    }
}

public class RemoveFolderEndpoint(IMediator mediator) : Endpoint<RemoveFolder.Request, Result>
{
    public override void Configure()
    {
        Delete("/folders/{FolderId}");
    }

    public override async Task HandleAsync(RemoveFolder.Request req, CancellationToken ct)
    {
        var result = await mediator.Send(new RemoveFolder.Command(req.FolderId), ct);

        if (result.IsFailure)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}

public class GetFolderImagesEndpoint(IMediator mediator) : Endpoint<GetFolderImages.Request, Result<GetFolderImages.Response>>
{
    public override void Configure()
    {
        Get("/folders/{FolderId}/images");
    }

    public override async Task HandleAsync(GetFolderImages.Request req, CancellationToken ct)
    {
        var result = await mediator.Send(new GetFolderImages.Command(req.FolderId), ct);

        if (result.IsFailure)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(result, cancellation: ct);
    }
}
