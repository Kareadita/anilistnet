using Kavita.AniListNet.Helpers;

namespace Kavita.AniListNet.Objects;

public class MediaEntryCollection
{
    [GqlSelection("lists")] public MediaEntryList[] Lists { get; private set; }
    [GqlSelection("hasNextChunk")] public bool HasNextChunk { get; private set; }
}
