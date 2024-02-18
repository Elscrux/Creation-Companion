using Avalonia.Controls;
using Avalonia.Input;
namespace CreationEditor.Avalonia.Services.Record.Actions;

public sealed class InjectedRecordContextMenuProvider : IRecordContextMenuProvider {
    private readonly Dictionary<RecordActionGroup, List<RecordAction>> _groups;
    private readonly Dictionary<KeyGesture, List<RecordAction>> _hotKeys = [];

    public InjectedRecordContextMenuProvider(IEnumerable<IRecordActionsProvider> recordActionsProvider) {
        _groups = recordActionsProvider
            .Select(p => p.GetActions())
            .SelectMany(x => x)
            .GroupBy(x => x.Group)
            .ToDictionary(x => x.Key, x => x.ToList());

        foreach (var actions in _groups
            .SelectMany(x => x.Value)
            .GroupBy(x => x.HotKeyFactory?.Invoke())) {
            if (actions.Key is null) continue;

            var recordActions = actions
                .OrderByDescending(x => x.Priority)
                .ToList();

            _hotKeys.Add(actions.Key, recordActions);
        }
    }

    public void TryToExecuteHotkey(KeyGesture keyGesture, Func<RecordListContext> contextFactory) {
        if (!_hotKeys.TryGetValue(keyGesture, out var recordActions)) return;

        var recordAction = recordActions.FirstOrDefault();
        if (recordAction is null) return;

        using var disposable = recordAction.Command.Execute(contextFactory()).Subscribe();
    }

    public IEnumerable<object> GetMenuItems(RecordListContext context) {
        return _groups.OrderByDescending(x => x.Key.Priority)
            // Get all the menu items for each group
            .Select(p => p.Value
                .Where(x => x.IsVisible(context))
                .OrderByDescending(x => x.Priority)
                .Select(action => action.MenuItemFactory(context)))
            // Flatten the groups into a single list and add separators between groups
            .SelectMany<IEnumerable<MenuItem>, object>(
                (x, i) => i == 0 ? x : [new Separator(), ..x]);
    }

    public void ExecutePrimary(RecordListContext context) {
        var primary = _groups
            .SelectMany(x => x.Value)
            .FirstOrDefault(x => x.IsPrimary && x.IsVisible(context));

        using var disposable = primary?.Command.Execute(context).Subscribe();
    }
}
