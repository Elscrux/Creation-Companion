using Avalonia.Controls;
using Avalonia.Input;
namespace CreationEditor.Avalonia.Services.Actions;

public sealed class InjectedContextMenuProvider : IContextMenuProvider {
    private readonly Dictionary<ContextActionGroup, List<ContextAction>> _groups;
    private readonly Dictionary<KeyGesture, List<ContextAction>> _hotKeys = [];

    public InjectedContextMenuProvider(IEnumerable<IContextActionsProvider> contextActionProviders) {
        _groups = contextActionProviders
            .Select(p => p.GetActions())
            .SelectMany(x => x)
            .GroupBy(x => x.Group)
            .ToDictionary(x => x.Key, x => x.ToList());

        foreach (var actions in _groups
            .SelectMany(x => x.Value)
            .GroupBy(x => x.HotKeyFactory?.Invoke())) {
            if (actions.Key is null) continue;

            var contextAction = actions
                .OrderByDescending(x => x.Priority)
                .ToList();

            _hotKeys.Add(actions.Key, contextAction);
        }
    }

    public void TryToExecuteHotkey(KeyGesture keyGesture, Func<SelectedListContext> contextFactory) {
        if (!_hotKeys.TryGetValue(keyGesture, out var contextActions)) return;

        var contextAction = contextActions.FirstOrDefault();
        if (contextAction is null) return;

        using var disposable = contextAction.Command?.Execute(contextFactory()).Subscribe();
    }

    public IEnumerable<object> GetMenuItems(SelectedListContext context) {
        var groups = _groups.OrderByDescending(x => x.Key.Priority)
            // Get all the menu items for each group
            .Select(p => p.Value
                .Where(x => x.IsVisible(context))
                .OrderByDescending(x => x.Priority)
                .Select(action => action.MenuItemFactory(context)));

        // Flatten the groups into a single list and add separators between groups
        var needSeparator = false;
        foreach (var group in groups) {
            if (!group.Any()) continue;

            if (needSeparator) {
                yield return new Separator();
            }

            foreach (var item in group) {
                yield return item;
            }

            needSeparator = true;
        }
    }

    public void ExecutePrimary(SelectedListContext context) {
        var primary = _groups
            .SelectMany(x => x.Value)
            .FirstOrDefault(x => x.IsPrimary && x.IsVisible(context));

        using var disposable = primary?.Command?.Execute(context).Subscribe();
    }
}
