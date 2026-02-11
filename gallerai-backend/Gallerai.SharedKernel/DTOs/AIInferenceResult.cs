using System.Text.Json.Serialization;

namespace Gallerai.SharedKernel.DTOs;

public class AIInferenceResult
{
    public AIInferenceResult(double score, string critique)
    {
        Score = score;
        Critique = critique;
    }

    public Guid ImageId { get; set; }

    [JsonPropertyName("score")]
    public double Score { get; }

    [JsonPropertyName("detailed_critique")]
    public string Critique { get; }
}
