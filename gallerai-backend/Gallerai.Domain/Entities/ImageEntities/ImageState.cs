using Gallerai.Domain.Entities.Abstract;
using Gallerai.Domain.Enums;

namespace Gallerai.Domain.Entities.ImageEntities;

public sealed class ImageState : ImageIdNavigationEntity
{
    private ImageState()
    {
    }

    public ImageState(ImageStatus status)
    {
        Status = status;
    }

    public ImageStatus Status { get; private set; }

    public void SetStatus(ImageStatus status)
    {
        Status = status;
    }
}
