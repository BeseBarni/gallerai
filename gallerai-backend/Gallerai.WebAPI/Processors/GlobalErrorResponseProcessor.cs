using FastEndpoints;
using Namotion.Reflection;
using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Gallerai.WebAPI.Processors;

public class GlobalErrorResponseProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var errorCodes = new[] { "400", "500" };

        foreach (var code in errorCodes)
        {
            if (!context.OperationDescription.Operation.Responses.ContainsKey(code))
            {
                // Generate the schema with a reference so it's reusable in Swagger UI
                var schema = context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema>(
                    typeof(ProblemDetails).ToContextualType(),
                    context.SchemaResolver);

                context.OperationDescription.Operation.Responses.Add(code, new OpenApiResponse
                {
                    Description = code == "400" ? "Bad Request" : "Internal Server Error",
                    Schema = schema
                });
            }
        }

        return true;
    }
}
