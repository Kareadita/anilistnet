using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Kavita.AniListNet.Helpers;
using Kavita.AniListNet.Objects;
using Newtonsoft.Json.Linq;

namespace Kavita.AniListNet;

public partial class AniClient(HttpClient client)
{
    private readonly Uri _url = new("https://graphql.anilist.co");

    public bool IsAuthenticated { get; private set; }
    /// <summary>
    /// The authenticated user. Null when <see cref="IsAuthenticated"/> is false. When true, may still be null
    /// if not loaded with <see cref="TryAuthenticateAsync"/>
    /// </summary>
    public User? AuthenticatedUser { get; private set; }

    public event EventHandler<AniRateEventArgs>? RateChanged;

    public AniClient() : this(new HttpClient())
    {
    }

    public void SetAuthenticationHeader(string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        IsAuthenticated = true;
    }

    /// <summary>
    /// Sets the authentication header, and loads <see cref="AuthenticatedUser"/> if successful.
    /// Exceptions other than <see cref="HttpStatusCode.Unauthorized"/> are rethrown.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<bool> TryAuthenticateAsync(string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        try
        {
            AuthenticatedUser = await GetAuthenticatedUserAsync();
            IsAuthenticated = true;
        }
        catch (AniException aniException)
        {
            if (aniException.StatusCode != HttpStatusCode.Unauthorized)
                throw;

            client.DefaultRequestHeaders.Authorization = null;
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

        var response = await client.PostAsync(_url, body, cancellationToken);

        var retryAfter = GetHeaderInt("Retry-After");
        var rateLimit = GetHeaderInt("X-RateLimit-Limit");
        var rateRemaining = GetHeaderInt("X-RateLimit-Remaining");
        var rateReset = GetHeaderInt("X-RateLimit-Reset");

        if (rateLimit.HasValue && rateRemaining.HasValue)
        {
            RateChanged?.Invoke(this, new AniRateEventArgs(
                rateLimit.Value,
                rateRemaining.Value,
                retryAfter,
                rateReset
            ));
        }

        var responseText = await response.Content.ReadAsStringAsync();
        var responseJson = JObject.Parse(responseText);

        if (!response.IsSuccessStatusCode)
            throw new AniException
            (
                responseJson["errors"]!.First!["message"]!.ToString(),
                bodyText ?? string.Empty,
                responseText,
                response.StatusCode
            );

        if (responseJson["errors"] != null)
        {
            var errorMessage = responseJson["errors"]!.First!["message"]?.ToString() ?? "Unknown GraphQL error";
            throw new AniException(errorMessage, bodyText ?? string.Empty, responseText, response.StatusCode);
        }

        return responseJson["data"]!;

        int? GetHeaderInt(string headerName)
        {
            return response.Headers.TryGetValues(headerName, out var values)
                   && int.TryParse(values.FirstOrDefault(), out var val) ? val : null;
        }
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
