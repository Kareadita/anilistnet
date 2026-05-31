using System.Runtime.Serialization;

namespace Kavita.AniListNet.Objects;

public enum UserSort
{
    [EnumMember(Value = "ID")] Id,
    [EnumMember(Value = "USERNAME")] Username,
    [EnumMember(Value = "WATCHED_TIME")] WatchedTime,
    [EnumMember(Value = "CHAPTERS_READ")] ChaptersRead,
    [EnumMember(Value = "SEARCH_MATCH")] Relevance
}
