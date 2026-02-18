using System.Text.Json.Serialization;
using Gallerai.SharedKernel.Enums;

namespace Gallerai.SharedKernel.DTOs;

public class ImageUpdateNotification
{
    public ImageUpdateNotification(double score, string critique, ImageStatus status)
    {
        Score = score;
        Critique = critique;
        Status = status;
    }
    public ImageUpdateNotification(Guid imageId, double score, string critique, ImageStatus status)
    {
        ImageId = imageId;
        Score = score;
        Critique = critique;
        Status = status;
    }
    public ImageUpdateNotification(ImageStatus status)
    {
        Status = status;
    }

    public ImageUpdateNotification()
    {

    }

    public Guid ImageId { get; set; }

    [JsonPropertyName("score")]
    public double? Score { get; set; }

    [JsonPropertyName("detailed_critique")]
    public string? Critique { get; set; }
    public ImageStatus Status { get; set; }
}
