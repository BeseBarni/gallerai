using FastEndpoints;
using FastEndpoints.Swagger;
using Gallerai.SharedKernel.Consts;
using Gallerai.WebAPI.Processors;

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
                s.OperationProcessors.Add(new GlobalErrorResponseProcessor());
            };

        });
    }

    public static IApplicationBuilder UseGalleraiFastEndpoints(this WebApplication app)
    {
        return app.UseFastEndpoints(c =>
        {
            c.Endpoints.RoutePrefix = "api";
            c.Endpoints.ShortNames = true;

            c.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
            {
                return new Microsoft.AspNetCore.Mvc.ProblemDetails
                {
                    Type = "https://gallerai.com/errors/validation",
                    Title = "Validation Error",
                    Status = statusCode,
                    Detail = "One or more validation failures occurred.",
                    Instance = ctx.Request.Path,
                    Extensions =
                {
                    // Map FluentValidation failures to a dictionary
                    ["errors"] = failures
                        .GroupBy(f => f.PropertyName)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Select(e => e.ErrorMessage).ToArray())
                }
                };
            };
        });
    }
}
