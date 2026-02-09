using Gallerai.Domain.Entities.Abstract;
using Gallerai.Domain.Enums;

namespace Gallerai.Domain.Entities.ImageEntities;

public sealed class Image : ImageIdEntity
{
    private readonly List<ImageEvent> _imageEvents = new();
    private readonly List<ImageTag> _imageTags = new();

    private Image()
    {
    }
    public string? R2Key { get; private set; }
    public long? Size { get; private set; }
    public DateTime? UploadedAt { get; private set; }

    public ImageState Status { get; private set; } = null!;
    public ImageAnalysis Analysis { get; private set; } = null!;
    public ImageMetadata Metadata { get; private set; } = null!;

    public IReadOnlyCollection<ImageEvent> ImageEvents => _imageEvents;
    public IReadOnlyCollection<ImageTag> ImageTags => _imageTags;

    public static Image Create(Guid guid)
    {
        var image = new Image();
        image.ImageId = guid;
        var status = ImageStatus.UPLOADING;
        image.SetStatus(new ImageState(status));
        image.AddEvent(new ImageEvent(status, DateTime.UtcNow));

        return image;
    }
    public string GetFullPath(string publicUrl)
    {
        if (R2Key is null) throw new InvalidOperationException("Storage key is not set.");

        return string.Join('/', publicUrl.TrimEnd('/'), R2Key);
    }
    public void SetStorageKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
        R2Key = key;
    }
    public void SetStatus(ImageState status)
    {
        Status = status ?? throw new ArgumentNullException(nameof(status));
    }

    public void SetAnalysis(ImageAnalysis analysis)
    {
        Analysis = analysis ?? throw new ArgumentNullException(nameof(analysis));
    }

    public void SetMetadata(ImageMetadata metadata)
    {
        Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
    }

    public void AddEvent(ImageEvent imageEvent)
    {
        if (imageEvent == null) throw new ArgumentNullException(nameof(imageEvent));
        _imageEvents.Add(imageEvent);
    }

    public void AddTag(ImageTag tag)
    {
        if (tag == null) throw new ArgumentNullException(nameof(tag));
        if (_imageTags.Contains(tag)) return;
        _imageTags.Add(tag);
    }

    public void RemoveTag(ImageTag tag)
    {
        if (tag == null) throw new ArgumentNullException(nameof(tag));
        _imageTags.Remove(tag);
    }

    public ImageEvent MarkAsUploaded(long size, DateTime uploadedAt)
    {
        Size = size;
        UploadedAt = uploadedAt;
        var status = ImageStatus.WAITING_FOR_ANALYSIS;
        Status.SetStatus(status);
        return new ImageEvent(ImageId, status, DateTime.UtcNow);
    }
}
