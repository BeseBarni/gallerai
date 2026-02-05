using Gallerai.Domain.Entities.Abstract;

namespace Gallerai.Domain.Entities.ImageEntities;

public sealed class ImageMetadata : ImageIdNavigationEntity
{
    private ImageMetadata()
    {
    }

    public ImageMetadata(string title, string description)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public CameraInfo? Camera { get; private set; }
    public ExposureSettings? Exposure { get; private set; }

    public void SetCamera(CameraInfo? camera) => Camera = camera;
    public void SetExposure(ExposureSettings? exposure) => Exposure = exposure;
}

public sealed record CameraInfo(
    string? Make,
    string? Model,
    string? LensModel,
    string? Software,
    DateTimeOffset? CapturedAt);

public sealed record ExposureSettings(
    int? Iso,
    double? Aperture,
    double? ShutterSpeedSeconds,
    double? FocalLengthMm,
    double? ExposureCompensation,
    FlashMode Flash,
    WhiteBalanceMode WhiteBalance);

public enum FlashMode { Unknown = 0, NotFired = 1, Fired = 2 }
public enum WhiteBalanceMode { Unknown = 0, Auto = 1, Daylight = 2, Cloudy = 3, Tungsten = 4, Fluorescent = 5, Shade = 6 }
