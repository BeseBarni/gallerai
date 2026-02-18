using Gallerai.Domain.Entities.Abstract;
using Gallerai.SharedKernel.Enums;

namespace Gallerai.Domain.Entities.ImageEntities;

public sealed class ImageEvent : ImageIdNavigationEntity
{
    private ImageEvent()
    {
    }

    public ImageEvent(ImageStatus status, DateTime lastUpdate, string? message = null)
    {
        ImageEventId = Guid.NewGuid();
        Status = status;
        LastUpdate = lastUpdate;
        Message = message;
    }

    public ImageEvent(Guid imageId, ImageStatus status, DateTime lastUpdate, string? message = null)
    {
        ImageId = imageId;
        ImageEventId = Guid.NewGuid();
        Status = status;
        LastUpdate = lastUpdate;
        Message = message;
    }

    public Guid ImageEventId { get; private set; }
    public DateTime LastUpdate { get; private set; }
    public ImageStatus Status { get; private set; }
    public string? Message { get; private set; }

    public void Update(ImageStatus status, DateTime updatedAt, string? message = null)
    {
        Status = status;
        LastUpdate = updatedAt;
        Message = message;
    }
}
