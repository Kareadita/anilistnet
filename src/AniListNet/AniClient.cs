using System.Net;
using System.Net.Http.Headers;
using System.Text;
using AniListNet.Helpers;
using AniListNet.Objects;
using Newtonsoft.Json.Linq;

namespace AniListNet;

public partial class AniClient
{
    private readonly HttpClient _client = new();
    private readonly Uri _url = new("https://graphql.anilist.co");

    public bool IsAuthenticated { get; private set; }
    public User? AuthenticatedUser { get; private set; }

    public event EventHandler<AniRateEventArgs>? RateChanged;

    /// <summary>
    /// Sets the authentication header, and loads <see cref="AuthenticatedUser"/> if successful.
    /// Exceptions other than <see cref="HttpStatusCode.Unauthorized"/> are rethrown.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<bool> TryAuthenticateAsync(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        try
        {
            AuthenticatedUser = await GetAuthenticatedUserAsync();
            IsAuthenticated = true;
        }
        catch (AniException aniException)
        {
            if (aniException.StatusCode != HttpStatusCode.Unauthorized)
                throw;

            _client.DefaultRequestHeaders.Authorization = null;
            IsAuthenticated = false;
            AuthenticatedUser = null;
        }

        return IsAuthenticated;
    }

    private async Task<JToken> PostRequestAsync(GqlSelection selection, bool isMutation = false, CancellationToken cancellationToken = default)
    {
        // Build selection
        var bodyJson = JObject.FromObject(new { query = (isMutation ? "mutation" : string.Empty) + selection });
        var bodyText = bodyJson["query"]!.ToObject<string>();
        var body = new StringContent(bodyJson.ToString(), Encoding.UTF8, "application/json");

        // Send request
        var response = await _client.PostAsync(_url, body, cancellationToken);

        // Parse response
        var responseText = await response.Content.ReadAsStringAsync();
        var responseJson = JObject.Parse(responseText);

        if (!response.IsSuccessStatusCode)
            throw new AniException
            (
                responseJson["errors"]!.First!["message"]!.ToString(),
                bodyText!,
                responseText,
                response.StatusCode
            );

        // Check rate limit
        response.Headers.TryGetValues("Retry-After", out var retryAfterValues);
        response.Headers.TryGetValues("X-RateLimit-Limit", out var rateLimitValues);
        response.Headers.TryGetValues("X-RateLimit-Remaining", out var rateRemainingValues);
        response.Headers.TryGetValues("X-RateLimit-Reset", out var rateResetValues);

        var retryAfterString = retryAfterValues?.FirstOrDefault();
        var rateLimitString = rateLimitValues?.FirstOrDefault();
        var rateRemainingString = rateRemainingValues?.FirstOrDefault();
        var rateResetString = rateResetValues?.FirstOrDefault();

        var retryAfterValidated = int.TryParse(retryAfterString, out var retryAfter);
        var rateLimitValidated = int.TryParse(rateLimitString, out var rateLimit);
        var rateRemainingValidated = int.TryParse(rateRemainingString, out var rateRemaining);
        var rateResetValidated = int.TryParse(rateResetString, out var rateReset);

        if (retryAfterValidated && rateLimitValidated && rateRemainingValidated && rateResetValidated)
            RateChanged?.Invoke(this, new AniRateEventArgs(rateLimit, rateRemaining, retryAfter, rateReset));
        else if (rateLimitValidated && rateRemainingValidated)
            RateChanged?.Invoke(this, new AniRateEventArgs(rateLimit, rateRemaining));

        return responseJson["data"]!;
    }

    private async Task<JToken> GetSingleDataAsync(params GqlSelection[] path)
    {
        // Build path to selection
        var selection = path[^1];
        for (var index = path.Length - 2; index >= 0; index--)
        {
            var newSelection = path[index];
            newSelection.Selections ??= new List<GqlSelection>();
            newSelection.Selections.Add(selection);
            selection = newSelection;
        }

        // Send request
        var token = await PostRequestAsync(selection);

        // Get value from path
        token = path.Aggregate(token, (current, item) => current![item.Name]!);

        return token!;
    }
}
