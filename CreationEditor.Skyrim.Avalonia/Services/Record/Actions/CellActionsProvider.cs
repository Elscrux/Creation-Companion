using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Actions;

public sealed partial class CellActionsProvider : IContextActionsProvider {
    private readonly IDockFactory _dockFactory;
    private readonly IDockableDefinition? _viewport;
    private readonly IList<ContextAction> _actions;

    public CellActionsProvider(
        IMenuItemProvider menuItemProvider,
        IDockableRegistry dockableRegistry,
        IDockFactory dockFactory) {
        _dockFactory = dockFactory;
        _viewport = dockableRegistry.GetByType(DockableTypes.Viewport);

        _actions = [
            new ContextAction(context => context is { SelectedRecords: [{ ReferencedRecord.Record: ICellGetter }], SelectedAssets.Count: 0 },
                100,
                ContextActionGroup.Viewing,
                ViewCommand,
                context => menuItemProvider.View(ViewCommand, context),
                () => menuItemProvider.View(ViewCommand).HotKey,
                true),
        ];
    }

    [ReactiveCommand]
    private async Task View(SelectedListContext context) {
        if (context.SelectedRecords[0].ReferencedRecord.Record is not ICellGetter cell) return;

        var cellLoadStrategy = context.GetSetting<ICellLoadStrategy>();

        try {
            if (_viewport is not null) await _dockFactory.Open(_viewport);
        } catch (Exception) {
            // ignored
        }

        cellLoadStrategy.LoadCell(cell);
    }

    public IEnumerable<ContextAction> GetActions() => _actions;
}
