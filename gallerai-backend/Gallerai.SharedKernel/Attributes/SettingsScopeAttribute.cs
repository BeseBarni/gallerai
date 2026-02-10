namespace Gallerai.SharedKernel.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SettingsScopeAttribute : Attribute
{
    public string Scope { get; }

    public SettingsScopeAttribute(string scope)
    {
        Scope = scope;
    }
}

public static class SettingsScopes
{
    public const string Api = "Gallerai.Infrastructure";
    public const string Worker = "Gallerai.Workers.InferenceWorker";
    public const string Shared = "Shared"; // Registered in all projects
}
