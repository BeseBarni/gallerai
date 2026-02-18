using FastEndpoints;
using FastEndpoints.Swagger;
using Gallerai.Application;
using Gallerai.SharedKernel.Consts;
using Gallerai.WebAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation;
using Wolverine;

var bld = WebApplication.CreateBuilder();
bld.Services.AddApplication();
bld.Services.AddFastEndpoints(o =>
{
    o.Assemblies = [typeof(SwaggerExtensions).Assembly];
});
bld.Services.AddGalleraiSwagger();
bld.Host.UseWolverine(opts =>
{
});
var app = bld.Build();

app.UseGalleraiFastEndpoints().UseSwaggerGen();

await app.StartAsync();

try
{
    var generator = app.Services.GetRequiredService<IOpenApiDocumentGenerator>();

    var doc = await generator.GenerateAsync(SwaggerConsts.ApiVersion);

    var json = doc.ToJson();

    var outputPath = Path.Combine(Directory.GetCurrentDirectory(), SwaggerConsts.SwaggerJsonFileName);

    await File.WriteAllTextAsync(outputPath, json);

    Console.WriteLine($"✅ Schema extracted to: {outputPath}");
}
finally
{
    await app.StopAsync();
}
