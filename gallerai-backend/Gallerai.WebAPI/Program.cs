using FastEndpoints;
using FastEndpoints.Swagger;
using Gallerai.Application;
using Gallerai.Infrastructure;
using Gallerai.Infrastructure.Notifications;
using Gallerai.WebAPI.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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

var app = builder.Build();

await app.UseApplyMigrations();

app.UseForwardedHeaders();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapHub<ImageNotificationsHub>("/hubs/images");

app.UseGalleraiFastEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerGen();
    app.UseSwaggerUi();
}

app.Run();

