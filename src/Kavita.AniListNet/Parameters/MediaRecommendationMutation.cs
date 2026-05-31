using Kavita.AniListNet.Helpers;
using Kavita.AniListNet.Objects;

namespace Kavita.AniListNet.Parameters;

public class MediaRecommendationMutation
{
    /// <summary>
    /// The ID of the media to recommend
    /// </summary>
    public int MediaRecommendationId { get; set; }

    /// <summary>
    /// The rating to give the recommendation.
    /// </summary>
    public MediaRecommendationRating Rating { get; set; } = MediaRecommendationRating.NoRating;

    internal IList<GqlParameter> ToParameters()
    {
        var parameters = new List<GqlParameter>
        {
            new("mediaRecommendationId", MediaRecommendationId),
            new("rating", Rating)
        };
        return parameters;
    }
}
