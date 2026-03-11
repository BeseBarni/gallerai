using OpenTelemetry.Context.Propagation;

namespace Gallerai.Application.Helpers;

public static class TelemetryHelpers
{
    public static PropagationContext GetParentContext(string traceParent)
    {
        return Propagators.DefaultTextMapPropagator.Extract(
                default,
                traceParent,
                (carrier, key) => key == "traceparent" ? [carrier] : Array.Empty<string>());
    }
}
