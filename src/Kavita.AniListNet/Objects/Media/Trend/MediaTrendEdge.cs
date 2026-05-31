using Kavita.AniListNet.Helpers;

namespace Kavita.AniListNet.Objects;

public class MediaTrendEdge
{
    [GqlSelection("node")] public MediaTrend Node { get; private set; }
}
