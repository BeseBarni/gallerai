using System.Text.Json.Serialization;

namespace Gallerai.SharedKernel.Events;

public record StartAIInferenceEvent(Guid Id, string UserId, string PublicUrl);
