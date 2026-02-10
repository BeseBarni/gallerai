using FastEndpoints;
using FastEndpoints.Swagger;
using Gallerai.Application;
using Gallerai.Infrastructure;
using Gallerai.Infrastructure.Notifications;
using Gallerai.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks();

builder.Services.AddFastEndpoints();
builder.Services.AddGalleraiSwagger();

builder.Services.AddOpenApi();
builder.Services.AddCors();
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

app.UseCors(policy => policy
    .WithOrigins("http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());

app.MapHub<ImageNotificationsHub>("/hubs/images");

app.Run();

