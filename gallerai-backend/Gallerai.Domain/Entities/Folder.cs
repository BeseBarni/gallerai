using Gallerai.Domain.Entities.ImageEntities;

namespace Gallerai.Domain.Entities;

public class Folder
{
    private readonly List<Image> _imageList = new();
    public Guid FolderId { get; set; }
    public string UserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime? DeletedAt { get; set; }
    public virtual IReadOnlyCollection<Image> ImageList => _imageList;
    public static Folder Create(string userId, string name)
    {
        return new Folder { FolderId = Guid.NewGuid(), UserId = userId, Name = name };
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Folder name cannot be empty.", nameof(newName));
        Name = newName;
    }

    public void RemoveFolder()
    {
        DeletedAt = DateTime.UtcNow;
        foreach (var image in _imageList)
        {
            image.MarkAsDeleted();
        }

    }
    public void AddImage(Image image)
    {
        ArgumentNullException.ThrowIfNull(image);
        if (image.UserId != UserId) throw new InvalidOperationException("Cannot add an image that belongs to a different user.");

        _imageList.Add(image);
    }

    public void AddImages(IEnumerable<Image> images)
    {
        if (!images.Any()) return;

        if (images.Any(image => image.UserId != UserId)) throw new InvalidOperationException("Cannot add images that belong to a different user.");

        _imageList.AddRange(images);
    }
    public void RemoveImage(Image image)
    {
        if (image is null) throw new ArgumentNullException(nameof(image));
        _imageList.Remove(image);
    }

    public void RemoveImages(IEnumerable<Image> images)
    {
        if (!images.Any()) return;
        foreach (var image in images)
        {
            _imageList.Remove(image);
        }
    }
}
