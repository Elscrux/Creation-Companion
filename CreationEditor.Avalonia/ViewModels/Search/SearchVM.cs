using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.HotKey;
using CreationEditor.Avalonia.Services.Search;
using CreationEditor.Services.Search;
using DynamicData;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Search;

/// <summary>
/// Toggleable scope filter chip of the search bar.
/// </summary>
public sealed partial class SelectableSearchScopeVM : ViewModel {
    public SearchScope Scope { get; }
    public string Title => Scope.GetDisplayName();

    [Reactive] public partial bool IsSelected { get; set; }

    public SelectableSearchScopeVM(SearchScope scope, bool isSelected = true) {
        Scope = scope;
        IsSelected = isSelected;
    }
}

/// <summary>
/// View model of the global search bar.
/// </summary>
public sealed partial class SearchVM : ViewModel {
    private static readonly TimeSpan ItemsChangedThrottle = TimeSpan.FromMilliseconds(500);

    private readonly ISearchService _searchService;
    private readonly ISearchRecentsService _recentsService;
    private readonly IContextMenuProvider _contextMenuProvider;
    private readonly List<ISearchItemSource> _sources = [];
    private readonly SourceCache<SearchItem, string> _items = new(x => x.Id);

    [Reactive] public partial string? SearchText { get; set; }
    [Reactive] public partial bool IsBusy { get; set; }
    [Reactive] public partial int MaxResultsPerCategory { get; set; } = 8;
    [Reactive] public partial int SearchDelayInMilliseconds { get; set; } = 150;

    public IObservableCollection<SelectableSearchScopeVM> Scopes { get; } = new ObservableCollectionExtended<SelectableSearchScopeVM>();
    public IObservableCollection<SearchCategoryResult> Results { get; } = new ObservableCollectionExtended<SearchCategoryResult>();

    [Reactive] public partial SearchItemVM? SelectedItem { get; private set; }

    private readonly Subject<Unit> _closeRequested = new();
    public IObservable<Unit> CloseRequested => _closeRequested;

    private readonly Subject<Unit> _openRequested = new();
    public IObservable<Unit> OpenRequested => _openRequested;

    private readonly IDisposable _searchSubscription;

    public SearchVM(
        IEnumerable<ISearchItemSource> sources,
        ISearchService searchService,
        ISearchRecentsService recentsService,
        IContextMenuProvider contextMenuProvider,
        IHotkeyService hotkeyService) {
        _searchService = searchService;
        _recentsService = recentsService;
        _contextMenuProvider = contextMenuProvider;

        hotkeyService.Register(new KeyGesture(Key.P, KeyModifiers.Control), () => _openRequested.OnNext(Unit.Default));

        Scopes.AddRange(Enum.GetValues<SearchScope>()
            .OrderBy(x => x.DisplayOrder())
            .Select(scope => new SelectableSearchScopeVM(scope)));

        foreach (var source in sources) {
            _sources.Add(source);
            source.Items
                .Subscribe(changes => _items.Edit(updater => updater.Clone(changes)))
                .DisposeWith(this);
        }

        var scopeSelectionChanged = Scopes
            .ToObservable()
            .Select(scope => scope.WhenAnyValue(x => x.IsSelected))
            .Merge()
            .Select(_ => Unit.Default);

        var textQuery = this.WhenAnyValue(x => x.SearchText)
            .CombineLatest(scopeSelectionChanged, (text, _) => text)
            .Throttle(_ => this.WhenAnyValue(x => x.SearchDelayInMilliseconds).Select(ms => TimeSpan.FromMilliseconds(ms)));

        // Re-run the current query when items arrive while the index is still building
        var itemsChanged = _items.Connect()
            .Throttle(ItemsChangedThrottle, RxSchedulers.TaskpoolScheduler)
            .Select(_ => SearchText);

        _searchSubscription = textQuery
            .Merge(itemsChanged)
            .Do(text => {
                IsBusy = true;

                // Recents replace search results on an empty query; clear stale categories
                if (string.IsNullOrWhiteSpace(text)) {
                    Results.Clear();
                    ResetSelection();
                }
            })
            .Select(text => ComputeResults(text).Materialize())
            .Switch()
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(
                notification => {
                    switch (notification.Kind) {
                        case NotificationKind.OnNext:
                            OnResultStreamed(notification.Value);
                            break;

                        case NotificationKind.OnCompleted:
                        case NotificationKind.OnError:
                            // All scopes of the current query are done
                            IsBusy = false;
                            break;
                    }
                },
                () => IsBusy = false)
            .DisposeWith(this);
    }

    private IObservable<SearchCategoryResult> ComputeResults(string? text) {
        if (string.IsNullOrWhiteSpace(text)) {
            var recents = GetRecentResults();
            return recents.Count == 0 ? Observable.Empty<SearchCategoryResult>() : Observable.Return(recents[0]);
        }

        var scopes = Scopes.Where(x => x.IsSelected).Select(x => x.Scope).ToHashSet();
        if (scopes.Count == 0) return Observable.Empty<SearchCategoryResult>();

        // Results stream in per scope: fast scopes appear first while slow scopes are still computing
        return _searchService
            .SearchStream(text, scopes, _items.Items, MaxResultsPerCategory)
            .Select(result => new SearchCategoryResult(
                result.Scope,
                result.Scope.GetDisplayName(),
                result.Items.Select(item => new SearchItemVM(item)).ToArray()));
    }

    private IReadOnlyList<SearchCategoryResult> GetRecentResults() {
        var recents = _recentsService.GetRecentIds()
            .Select(id => _items.Lookup(id))
            .Where(x => x.HasValue)
            .Select(x => new SearchItemVM(x.Value))
            .Take(5)
            .ToArray();
        if (recents.Length == 0) return [];

        return [new SearchCategoryResult(SearchScope.Action, "Recent", recents, true)];
    }

    /// <summary>
    /// Merge a single streamed category into the results, keeping the display order of the scopes.
    /// </summary>
    /// <param name="category">Category to merge</param>
    private void OnResultStreamed(SearchCategoryResult category) {
        var existingIndex = -1;
        for (var i = 0; i < Results.Count; i++) {
            if (Results[i].Scope != category.Scope) continue;

            existingIndex = i;
            break;
        }

        if (existingIndex >= 0) {
            if (category.Items.Count == 0) {
                Results.RemoveAt(existingIndex);
            } else {
                Results[existingIndex] = category;
            }
        } else {
            if (category.Items.Count == 0) return;

            var insertIndex = 0;
            while (insertIndex < Results.Count && Results[insertIndex].Scope.DisplayOrder() < category.Scope.DisplayOrder()) {
                insertIndex++;
            }

            Results.Insert(insertIndex, category);
        }

        PreserveOrResetSelection();
    }

    private void PreserveOrResetSelection() {
        if (SelectedItem is not null && Results.SelectMany(x => x.Items).Contains(SelectedItem)) return;

        ResetSelection();
    }

    private void PublishResults(IReadOnlyList<SearchCategoryResult> categories) {
        Results.LoadOptimized(categories);
        ResetSelection();
        IsBusy = false;
    }

    private void ResetSelection() {
        SelectedItem?.SetSelected(false);
        SelectedItem = Results.SelectMany(x => x.Items).FirstOrDefault();
        SelectedItem?.SetSelected(true);
    }

    /// <summary>
    /// Move the selection to the next or previous item in the results.
    /// </summary>
    /// <param name="direction">Number of items to move, negative to move up</param>
    public void MoveSelection(int direction) {
        var items = Results.SelectMany(x => x.Items).ToList();
        if (items.Count == 0) return;

        var index = SelectedItem is null ? -1 : items.IndexOf(SelectedItem);
        var next = (index + direction + items.Count) % items.Count;

        SelectedItem?.SetSelected(false);
        SelectedItem = items[next];
        SelectedItem.SetSelected(true);
    }

    /// <summary>
    /// Execute the currently selected item.
    /// </summary>
    public void ExecuteSelected() {
        if (SelectedItem is null) return;

        _ = ExecuteItem(SelectedItem);
    }

    /// <summary>
    /// Clear the recents and refresh the results.
    /// </summary>
    public void ClearRecents() {
        _recentsService.Clear();

        // Recents are only visible on an empty query, and computing them needs no background work
        if (!string.IsNullOrWhiteSpace(SearchText)) return;

        PublishResults(GetRecentResults());
    }

    /// <summary>
    /// Execute a search item and record it as recent.
    /// </summary>
    /// <param name="item">Item to execute</param>
    public async Task ExecuteItem(SearchItemVM item) {
        _recentsService.Add(item.Item.Id);
        await item.ExecuteCommand.Execute();
    }

    /// <summary>
    /// Get the context menu items of a search item, if it has context actions.
    /// </summary>
    /// <param name="item">Item to get the context menu items for</param>
    /// <returns>Context menu items, or null if the item has no context actions</returns>
    public IEnumerable<Control>? GetContextMenuItems(SearchItemVM item) {
        var context = item.Item.ContextFactory?.Invoke();
        if (context is not SelectedListContext selectedListContext) return null;

        return _contextMenuProvider.GetMenuItems(selectedListContext).ToArray();
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);

        _searchSubscription.Dispose();
        _closeRequested.Dispose();
        _openRequested.Dispose();
        _items.Dispose();
        foreach (var source in _sources) {
            source.Dispose();
        }
    }
}
