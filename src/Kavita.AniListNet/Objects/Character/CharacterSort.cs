using System.Runtime.Serialization;

namespace Kavita.AniListNet.Objects;

public enum CharacterSort
{
    [EnumMember(Value = "ID")] Id,
    [EnumMember(Value = "ROLE")] Role,
    [EnumMember(Value = "SEARCH_MATCH")] Relevance,
    [EnumMember(Value = "FAVOURITES")] Favorites
}
