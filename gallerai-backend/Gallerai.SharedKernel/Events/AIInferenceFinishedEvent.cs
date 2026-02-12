using Gallerai.SharedKernel.DTOs;

namespace Gallerai.SharedKernel.Events;

public record AIInferenceFinishedEvent(Guid ImageId, string UserId, bool IsSuccess, AIInferenceResult? Result);
