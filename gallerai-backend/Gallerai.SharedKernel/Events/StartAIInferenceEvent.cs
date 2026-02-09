using System.Text.Json.Serialization;

namespace Gallerai.SharedKernel.Events;

public record StartAIInferenceEvent(Guid Id, [property: JsonPropertyName("publicUrl")] string PublicUrl);
