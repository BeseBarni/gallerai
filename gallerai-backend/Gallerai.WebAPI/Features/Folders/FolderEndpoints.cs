using FastEndpoints;
using Gallerai.Application.Features.Folders;
using Gallerai.SharedKernel.Models;
using Gallerai.WebAPI.Extensions;
using Wolverine;

namespace Gallerai.WebAPI.Features.Folders;

public class GetFoldersEndpoint(IMessageBus bus) : EndpointWithoutRequest<GetFolders.Response>
{
    public override void Configure()
    {
        Get("/folders");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<GetFolders.Response>>(new GetFolders.Command(), ct);

        await this.HandleResultAsync(result, ct);
    }
}

public class AddFolderEndpoint(IMessageBus bus) : Endpoint<AddFolder.Request, AddFolder.Response>
{
    public override void Configure()
    {
        Post("/folders");
    }

    public override async Task HandleAsync(AddFolder.Request req, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<AddFolder.Response>>(new AddFolder.Command(req.Name), ct);

        await this.HandleCreatedAsync(
            result,
            nameof(GetFolderByIdEndpoint),
            new { result.Value?.FolderId },
            ct);
    }
}

public class RenameFolderEndpoint(IMessageBus bus) : Endpoint<RenameFolder.Request, RenameFolder.Response>
{
    public override void Configure()
    {
        Put("/folders/{FolderId}");
    }

    public override async Task HandleAsync(RenameFolder.Request req, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<RenameFolder.Response>>(new RenameFolder.Command(req.FolderId, req.NewName), ct);

        await this.HandleResultAsync(result, ct);
    }
}

public class RemoveFolderEndpoint(IMessageBus bus) : Endpoint<RemoveFolder.Request, object>
{
    public override void Configure()
    {
        Delete("/folders/{FolderId}");
    }

    public override async Task HandleAsync(RemoveFolder.Request req, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result>(new RemoveFolder.Command(req.FolderId), ct);

        await this.HandleNoContentResultAsync(result, ct);
    }
}

public class GetFolderByIdEndpoint(IMessageBus bus) : Endpoint<GetFolderById.Request, GetFolderById.Response>
{
    public override void Configure()
    {
        Get("/folders/{FolderId}");
    }

    public override async Task HandleAsync(GetFolderById.Request req, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<GetFolderById.Response>>(new GetFolderById.Command(req.FolderId), ct);

        await this.HandleResultAsync(result, ct);
    }
}

public class GetFolderImagesEndpoint(IMessageBus bus) : Endpoint<GetFolderImages.Request, GetFolderImages.Response>
{
    public override void Configure()
    {
        Get("/folders/{FolderId}/images");
    }

    public override async Task HandleAsync(GetFolderImages.Request req, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<Result<GetFolderImages.Response>>(new GetFolderImages.Command(req.FolderId), ct);

        await this.HandleResultAsync(result, ct);
    }
}
