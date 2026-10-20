using System.Reactive.Concurrency;
using System.Reactive.Linq;
namespace CreationEditor.Services.Search;

public sealed record SearchCategoryResult(SearchScope Scope, IReadOnlyList<SearchItem> Items);

/// <summary>
/// Aggregates search items into grouped, ranked results.
/// </summary>
public interface ISearchService {
    /// <summary>
    /// Search the given items for the query.
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="scopes">Scopes to include in the results</param>
    /// <param name="items">Items to search through</param>
    /// <param name="maxPerCategory">Maximum amount of results per scope category</param>
    /// <returns>Results grouped by scope, ordered by display order of the scopes</returns>
    IReadOnlyList<SearchCategoryResult> Search(string query, IReadOnlySet<SearchScope> scopes, IEnumerable<SearchItem> items, int maxPerCategory = 8);

    /// <summary>
    /// Search the given items for the query and stream the result of each scope as soon as it is computed.
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="scopes">Scopes to include in the results</param>
    /// <param name="items">Items to search through</param>
    /// <param name="maxPerCategory">Maximum amount of results per scope category</param>
    /// <returns>One result per selected scope, as soon as it is computed; scopes without matches emit an empty result</returns>
    IObservable<SearchCategoryResult> SearchStream(string query, IReadOnlySet<SearchScope> scopes, IEnumerable<SearchItem> items, int maxPerCategory = 8);
}

public sealed class SearchService(ISearchRanker ranker) : ISearchService {
    private const double TitleWeight = 1.0;
    private const double SecondaryWeight = 0.5;

    public IReadOnlyList<SearchCategoryResult> Search(string query, IReadOnlySet<SearchScope> scopes, IEnumerable<SearchItem> items, int maxPerCategory = 8) {
        if (string.IsNullOrWhiteSpace(query)) return [];

        var buckets = BucketItems(scopes, items);

        var results = new List<SearchCategoryResult>();
        foreach (var scope in buckets.Keys.OrderBy(x => x.DisplayOrder())) {
            var result = RankScope(query, scope, buckets[scope], maxPerCategory);
            if (result is not null) results.Add(result);
        }

        return results;
    }

    public IObservable<SearchCategoryResult> SearchStream(string query, IReadOnlySet<SearchScope> scopes, IEnumerable<SearchItem> items, int maxPerCategory = 8) {
        if (string.IsNullOrWhiteSpace(query)) return Observable.Empty<SearchCategoryResult>();

        // Bucket all items in one background pass, then rank each scope independently
        // and emit its results as soon as they are computed.
        // Every selected scope emits a result, even an empty one, so consumers can clear stale results.
        return Observable
            .Start(() => BucketItems(scopes, items), TaskPoolScheduler.Default)
            .SelectMany(buckets => scopes
                .OrderBy(x => x.DisplayOrder())
                .Select(scope => Observable
                    .Start(
                        () => buckets.TryGetValue(scope, out var bucket)
                            ? RankScope(query, scope, bucket, maxPerCategory)
                            : null,
                        TaskPoolScheduler.Default)
                    .Select(result => result ?? new SearchCategoryResult(scope, []))))
            .Merge();
    }

    private static Dictionary<SearchScope, List<SearchItem>> BucketItems(IReadOnlySet<SearchScope> scopes, IEnumerable<SearchItem> items) {
        var buckets = new Dictionary<SearchScope, List<SearchItem>>();
        foreach (var item in items) {
            if (!scopes.Contains(item.Scope)) continue;

            if (!buckets.TryGetValue(item.Scope, out var bucket)) {
                bucket = [];
                buckets[item.Scope] = bucket;
            }

            bucket.Add(item);
        }

        return buckets;
    }

    private SearchCategoryResult? RankScope(string query, SearchScope scope, IReadOnlyList<SearchItem> items, int maxPerCategory) {
        var ranked = new List<(SearchItem Item, double Score)>();
        foreach (var item in items) {
            var bestMatch = GetBestMatch(item, query);
            if (bestMatch is null) continue;

            ranked.Add((item, bestMatch.Score));
        }

        if (ranked.Count == 0) return null;

        return new SearchCategoryResult(
            scope,
            ranked
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Item.Priority)
                .ThenBy(x => x.Item.Title, StringComparer.OrdinalIgnoreCase)
                .Take(maxPerCategory)
                .Select(x => x.Item)
                .ToList());
    }

    private SearchMatch? GetBestMatch(SearchItem item, string query) {
        SearchMatch? best = null;
        var isFirst = true;
        foreach (var content in item.GetSearchableContent()) {
            var weight = isFirst ? TitleWeight : SecondaryWeight;
            isFirst = false;

            var match = ranker.Match(content, query, weight);
            if (match is not null && (best is null || match.Score > best.Score)) best = match;
        }

        return best;
    }
}
