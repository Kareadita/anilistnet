using Kavita.AniListNet.Helpers;

namespace Kavita.AniListNet.Objects;

public class MediaList
{
    [GqlSelection("name")] public string Name { get; private set; }
    [GqlSelection("isCustomList")] public string IsCustom { get; private set; }
    [GqlSelection("status")] public MediaEntryStatus? Status { get; private set; }
}
