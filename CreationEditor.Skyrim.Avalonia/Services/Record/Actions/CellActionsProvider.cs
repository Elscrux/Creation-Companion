using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Actions;

public sealed partial class CellActionsProvider : IContextActionsProvider {
    private readonly IDockFactory _dockFactory;
    private readonly IList<ContextAction> _actions;

    public CellActionsProvider(
        IMenuItemProvider menuItemProvider,
        IDockFactory dockFactory) {
        _dockFactory = dockFactory;

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
            await _dockFactory.Open(DockElement.Viewport);
        } catch (Exception) {
            // ignored
        }

        cellLoadStrategy.LoadCell(cell);
    }

    public IEnumerable<ContextAction> GetActions() => _actions;
}
