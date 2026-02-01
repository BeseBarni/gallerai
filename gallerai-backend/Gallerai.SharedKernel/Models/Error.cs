namespace Gallerai.SharedKernel.Models;

public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");
    public static Error NotFound(string entityName, object id) =>
        new("Error.NotFound", $"{entityName} with id {id} was not found.");
}
