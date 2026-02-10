namespace Gallerai.SharedKernel.Events;

public record AIInferenceFinishedEvent(Guid imageId, string result);
