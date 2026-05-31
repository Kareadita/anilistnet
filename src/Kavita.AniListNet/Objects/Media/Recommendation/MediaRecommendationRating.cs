using System.Runtime.Serialization;

namespace Kavita.AniListNet.Objects;

public enum MediaRecommendationRating
{
    [EnumMember(Value = "NO_RATING")] NoRating,
    [EnumMember(Value = "RATE_UP")] RateUp,
    [EnumMember(Value = "RATE_DOWN")] RateDown
}
