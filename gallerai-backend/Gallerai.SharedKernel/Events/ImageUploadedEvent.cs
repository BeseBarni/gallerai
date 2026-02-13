namespace Gallerai.SharedKernel.Events;

public record ImageUploadedEvent(Guid ImageId, long Size, DateTime TimeStamp);
