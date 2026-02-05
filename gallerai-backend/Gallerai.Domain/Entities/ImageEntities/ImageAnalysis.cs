using Gallerai.Domain.Entities.Abstract;

namespace Gallerai.Domain.Entities.ImageEntities;

public sealed class ImageAnalysis : ImageIdNavigationEntity
{
    private ImageAnalysis()
    {
    }

    public ImageAnalysis(double aestheticScore, string critique, IReadOnlyCollection<string>? labels = null)
    {
        AestheticScore = aestheticScore;
        Critique = critique ?? throw new ArgumentNullException(nameof(critique));
    }

    public double AestheticScore { get; private set; }
    public string Critique { get; private set; } = null!;
    public void UpdateScore(double aestheticScore) => AestheticScore = aestheticScore;
    public void UpdateCritique(string critique) => Critique = critique ?? throw new ArgumentNullException(nameof(critique));
}
