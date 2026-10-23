using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using CreationEditor.Avalonia.Converter;
using CreationEditor.Services.Search;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Search;

/// <summary>
/// Category of search results, rendered as a header with its items listed beneath.
/// </summary>
/// <param name="Title">Header title, e.g. "Records" or "Recent"</param>
/// <param name="Items">Items of the category, listed beneath the header</param>
/// <param name="IsClearable">True if the category can be cleared, e.g. the recents</param>
public sealed record SearchCategoryResult(SearchScope Scope, string Title, IReadOnlyList<SearchItemVM> Items, bool IsClearable = false);

/// <summary>
/// Display wrapper of a <see cref="SearchItem"/> for the search results.
/// </summary>
public sealed partial class SearchItemVM : ViewModel {
    private readonly SearchItem _item;

    public string Title => _item.Title;
    public string? Subtitle => _item.Subtitle;
    public SearchScope Scope => _item.Scope;
    public Control? IconControl { get; }
    public bool HasContextActions { get; }
    public SearchItem Item => _item;

    [Reactive] public partial bool IsEnabled { get; set; }
    [Reactive] public partial bool IsSelected { get; private set; }
    public ReactiveCommand<Unit, Unit> ExecuteCommand { get; }

    public SearchItemVM(SearchItem item) {
        _item = item;

        IconControl = Dispatcher.UIThread.Invoke(() => IconConverters.ToIcon(item.Icon));
        HasContextActions = item.ContextFactory is not null;

        var canExecute = item.CanExecute ?? Observable.Return(true);
        canExecute
            .ObserveOnGui()
            .Subscribe(isEnabled => IsEnabled = isEnabled)
            .DisposeWith(this);

        ExecuteCommand = ReactiveCommand.Create(Execute, canExecute);
    }

    public void SetSelected(bool isSelected) => IsSelected = isSelected;

    private void Execute() {
        if (_item.Execute is null) return;

        _ = _item.Execute();
    }
}
