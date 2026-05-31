using Kavita.AniListNet.Helpers;

namespace Kavita.AniListNet.Objects;

public class StudioEdge
{
    [GqlSelection("node")] public Studio Studio { get; private set; }
    [GqlSelection("id")] public int Id { get; private set; }
    [GqlSelection("isMain")] public bool IsMain { get; private set; }
}
