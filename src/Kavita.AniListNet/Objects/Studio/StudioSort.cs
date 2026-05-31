using System.Runtime.Serialization;

namespace Kavita.AniListNet.Objects;

public enum StudioSort
{
    [EnumMember(Value = "ID")] Id,
    [EnumMember(Value = "NAME")] Name,
    [EnumMember(Value = "SEARCH_MATCH")] Relevance,
    [EnumMember(Value = "FAVOURITES")] Favorites
}
