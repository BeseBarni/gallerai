using FastEndpoints;

namespace Gallerai.WebAPI.Endpoints.Hello;

public class HelloResponse
{
    public string Message { get; set; } = default!;
}

public class HelloEndpoint : EndpointWithoutRequest<HelloResponse>
{
    public override void Configure()
    {
        Get("/hello");
        AllowAnonymous();
        Summary(s => s.Summary = "Simple hello-world test endpoint");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync(new HelloResponse { Message = "hello world" }, cancellation: ct);
    }
}
