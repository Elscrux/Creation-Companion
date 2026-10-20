using System.Reactive.Linq;
using CreationEditor.Services.Search;
namespace CreationEditor.Skyrim.Tests.Search;

public sealed class SearchServiceTests {
    private readonly SearchService _searchService = new(new FuzzySearchRanker());

    private static SearchItem CreateItem(string title, SearchScope scope, int priority = 0) {
        return new SearchItem(
            title,
            title,
            scope,
            Priority: priority,
            Execute: () => Task.CompletedTask);
    }

    [Fact]
    public void ResultsAreGroupedByScope() {
        var items = new[] {
            CreateItem("Iron Sword", SearchScope.Record),
            CreateItem("textures/IronSword.dds", SearchScope.Asset),
            CreateItem("Open Log", SearchScope.Action),
            CreateItem("Save", SearchScope.Action),
        };

        var results = _searchService.Search("Iron", AllScopes, items);

        var scopes = results.Select(x => x.Scope).ToList();
        Assert.Equal([SearchScope.Record, SearchScope.Asset], scopes);
        Assert.All(results.SelectMany(x => x.Items), item => Assert.NotEqual("Save", item.Title));
    }

    [Fact]
    public void ScopeOrderFollowsDisplayOrder() {
        var items = new[] {
            CreateItem("Save Open Changes", SearchScope.Action),
            CreateItem("Open Log", SearchScope.Asset),
            CreateItem("Open Chests", SearchScope.Record),
        };

        var results = _searchService.Search("Open", AllScopes, items);

        // Only matching scopes are returned, in display order
        var scopes = results.Select(x => x.Scope).ToList();
        Assert.Equal([SearchScope.Record, SearchScope.Asset, SearchScope.Action], scopes);
    }

    [Fact]
    public void ScopeFilterIsHonored() {
        var items = new[] {
            CreateItem("Iron Sword", SearchScope.Record),
            CreateItem("Open Record Browser", SearchScope.Asset),
        };

        var results = _searchService.Search("Record", new HashSet<SearchScope> { SearchScope.Asset }, items);

        Assert.Single(results);
        Assert.Equal(SearchScope.Asset, results[0].Scope);
    }

    [Fact]
    public void MaxPerCategoryIsHonored() {
        var items = Enumerable.Range(0, 20)
            .Select(i => CreateItem($"Iron Sword {i}", SearchScope.Record))
            .ToList();

        var results = _searchService.Search("Iron", AllScopes, items, 5);

        Assert.Single(results);
        Assert.Equal(5, results[0].Items.Count);
    }

    [Fact]
    public void SearchStreamYieldsPerScopeResults() {
        var items = new[] {
            CreateItem("Iron Sword", SearchScope.Record),
            CreateItem("Iron Dagger", SearchScope.Record),
            CreateItem("Open Iron Browser", SearchScope.Asset),
        };

        var results = _searchService.SearchStream("Iron", AllScopes, items).ToEnumerable().ToList();

        // Results stream in per scope as soon as they are computed; emission order is not guaranteed
        Assert.Equal(AllScopes.Count, results.Count);
        Assert.Equal(2, results.Single(x => x.Scope == SearchScope.Record).Items.Count);
        Assert.Single(results.Single(x => x.Scope == SearchScope.Asset).Items);
        Assert.All(results.Where(x => x.Scope is not (SearchScope.Record or SearchScope.Asset)), x => Assert.Empty(x.Items));
    }

    [Fact]
    public void SearchStreamEmitsEmptyResultsForEmptyScopes() {
        var items = new[] {
            CreateItem("Save", SearchScope.Action),
        };

        var results = _searchService.SearchStream("Iron", AllScopes, items).ToEnumerable().ToList();

        // Every selected scope emits a result, empty ones included, so stale results can be cleared
        Assert.Equal(AllScopes.Count, results.Count);
        Assert.All(results, x => Assert.Empty(x.Items));
    }

    [Fact]
    public void EmptyQueryReturnsNoResults() {
        var items = new[] { CreateItem("Save", SearchScope.Action) };

        Assert.Empty(_searchService.Search("", AllScopes, items));
        Assert.Empty(_searchService.Search("   ", AllScopes, items));
    }

    [Fact]
    public void BetterMatchesComeFirst() {
        var items = new[] {
            CreateItem("Save All", SearchScope.Action),
            CreateItem("Save", SearchScope.Action),
        };

        var results = _searchService.Search("Save", AllScopes, items);

        Assert.Equal("Save", results[0].Items[0].Title);
        Assert.Equal("Save All", results[0].Items[1].Title);
    }

    [Fact]
    public void KeywordsAreSearchable() {
        var item = new SearchItem(
            "XYZ",
            "Mysterious Item",
            SearchScope.Record,
            Keywords: ["WeaponSword"],
            Execute: () => Task.CompletedTask);

        var results = _searchService.Search("Weapon", AllScopes, [item]);

        Assert.Single(results);
    }

    private static IReadOnlySet<SearchScope> AllScopes { get; } = new HashSet<SearchScope>(Enum.GetValues<SearchScope>());
}
