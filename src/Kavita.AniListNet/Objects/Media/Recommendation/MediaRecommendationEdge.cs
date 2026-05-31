using Kavita.AniListNet.Helpers;

namespace Kavita.AniListNet.Objects;

public class MediaRecommendationEdge
{
    [GqlSelection("node")] public MediaRecommendation Recommendation { get; private set; }
}
