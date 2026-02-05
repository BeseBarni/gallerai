using FastEndpoints;
using FastEndpoints.Swagger;
using Gallerai.Application;
using Gallerai.Infrastructure;
using Gallerai.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks();

builder.Services.AddFastEndpoints();
builder.Services.AddGalleraiSwagger();

builder.Services.AddOpenApi();

var app = builder.Build();

await app.UseApplyMigrations();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerGen();
    app.UseSwaggerUi();
}

app.MapHealthChecks("/health");

app.UseGalleraiFastEndpoints();
app.Run();

