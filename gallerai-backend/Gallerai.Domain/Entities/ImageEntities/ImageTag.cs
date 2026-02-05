namespace Gallerai.Domain.Entities.ImageEntities;

public class ImageTag
{
    public Guid Id { get; set; }
    public string Tag { get; set; } = null!;
    public ICollection<Image> ImageList { get; set; } = new HashSet<Image>();
}
