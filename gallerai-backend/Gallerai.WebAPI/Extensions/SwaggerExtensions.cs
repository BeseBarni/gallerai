using FastEndpoints;
using FastEndpoints.Swagger;
using Gallerai.SharedKernel.Consts;

namespace Gallerai.WebAPI.Extensions;

public static class SwaggerExtensions
{
    public static void AddGalleraiSwagger(this IServiceCollection services)
    {
        services.SwaggerDocument(o =>
        {
            o.DocumentSettings = s =>
            {
                s.Title = SwaggerConsts.ApiTitle;
                s.Version = SwaggerConsts.ApiVersion;
                s.Description = SwaggerConsts.ApiDescription;
                o.ShortSchemaNames = true;
                o.RemoveEmptyRequestSchema = true;
            };
        });
    }

    public static IApplicationBuilder UseGalleraiFastEndpoints(this WebApplication app)
    {
        return app.UseFastEndpoints(c =>
        {
            c.Endpoints.RoutePrefix = "api";
            c.Endpoints.ShortNames = true;
        });
    }
}
