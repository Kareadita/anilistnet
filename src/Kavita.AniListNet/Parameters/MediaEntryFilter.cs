using Kavita.AniListNet.Helpers;
using Kavita.AniListNet.Objects;

namespace Kavita.AniListNet.Parameters;

public class MediaEntryFilter
{
    public MediaType? Type { get; set; }
    public MediaEntryStatus? Status { get; set; }
    public MediaEntrySort Sort { get; set; } = MediaEntrySort.LastUpdated;
    public bool SortDescending { get; set; } = true;

    internal IList<GqlParameter> ToParameters()
    {
        var parameters = new List<GqlParameter>();
        if (Type.HasValue)
            parameters.Add(new GqlParameter("type", Type));
        if (Status.HasValue)
            parameters.Add(new GqlParameter("status", Status));
        parameters.Add(new GqlParameter("sort",
            $"${HelperUtilities.GetEnumMemberValue(Sort)}" + (SortDescending ? "_DESC" : string.Empty)));
        return parameters;
    }
}
