namespace Gallerai.Domain.Entities.Abstract;

public class ImageIdNavigationEntity : ImageIdEntity
{
    public virtual ImageEntities.Image Image { get; set; } = null!;
}
