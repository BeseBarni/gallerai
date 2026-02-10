using Gallerai.SharedKernel.DTOs;

namespace Gallerai.SharedKernel.Events;

public record AIInferenceFinishedEvent(Guid imageId, bool isSuccess, AIInferenceResult? result);
