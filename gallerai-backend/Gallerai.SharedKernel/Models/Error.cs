namespace Gallerai.SharedKernel.Models;

public sealed record Error(string Code, string Message, int StatusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, 200);
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.", 400);
    public static Error NotFound(string entityName, object id) =>
        new("Error.NotFound", $"{entityName} with id {id} was not found.", 404);
}
