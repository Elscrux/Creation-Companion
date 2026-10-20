using CreationEditor.Avalonia.Services.Actions.Global;
using CreationEditor.Services.Search;
using DynamicData;
namespace CreationEditor.Avalonia.Services.Search;

/// <summary>
/// Surfaces all registered global actions as search items.
/// </summary>
public sealed class GlobalActionSearchSource(GlobalActionRegistry registry) : ISearchItemSource {
    public IObservable<IChangeSet<SearchItem, string>> Items => registry.Actions.Connect().Transform(CreateItem);

    private static SearchItem CreateItem(GlobalAction action) {
        return new SearchItem(
            action.Id,
            action.Title,
            SearchScope.Action,
            action.Subtitle,
            action.Icon,
            action.Priority,
            Execute: action.Command is null
                ? null
                : () => {
                    using var disposable = action.Command.Execute().Subscribe();
                    return Task.CompletedTask;
                },
            CanExecute: action.Command?.CanExecute);
    }

    public void Dispose() {}
}
