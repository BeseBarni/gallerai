using FastEndpoints;
using FastEndpoints.Swagger;
using Gallerai.Application;
using Gallerai.Application.Behaviors;
using Gallerai.Infrastructure;
using Gallerai.SignalR.Shared.Consts;
using Gallerai.SignalR.Shared.Hubs;
using Gallerai.WebAPI.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(Gallerai.Application.DependencyInjection).Assembly);
    opts.Policies
    .ForMessagesOfType<IUserRequest>()
    .AddMiddleware(typeof(UserIdMiddleware));
});

builder.Services.AddHealthChecks();

builder.Services.AddFastEndpoints();
builder.Services.AddGalleraiSwagger();

builder.Services.AddOpenApi();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true;
    options.IncludeScopes = true;
});

var app = builder.Build();

await app.UseApplyMigrations();

app.UseForwardedHeaders();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapHub<ImageNotificationsHub>(HubConsts.ImagesHub);

app.UseGalleraiFastEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerGen();
    app.UseSwaggerUi();
}

app.Run();

