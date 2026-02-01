using FastEndpoints;
using FastEndpoints.Swagger;
using Gallerai.Application;
using Gallerai.Infrastructure;
using Gallerai.WebAPI.Extensions;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks();

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Gallerai API";
        s.Version = "v1";
        s.Description = "API for Gallerai system";
    };
});

builder.Services.AddOpenApi();

var app = builder.Build();

await app.UseApplyMigrations();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapHealthChecks("/health");
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
});

app.Run();

