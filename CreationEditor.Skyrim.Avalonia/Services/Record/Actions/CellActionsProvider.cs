using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Actions;

public sealed class CellActionsProvider : IContextActionsProvider {
    private readonly IList<ContextAction> _actions;

    public CellActionsProvider(
        IMenuItemProvider menuItemProvider,
        IDockFactory dockFactory) {

        var viewCommand = ReactiveCommand.CreateFromTask<SelectedListContext>(async context => {
            if (context.SelectedRecords[0].Record is not ICellGetter cell) return;

            var cellLoadStrategy = context.GetSetting<ICellLoadStrategy>();

            try {
                await dockFactory.Open(DockElement.Viewport);
            } catch (Exception) {
                // ignored
            }

            cellLoadStrategy.LoadCell(cell);
        });

        _actions = [
            new ContextAction(context => context is { SelectedRecords: [{ Record: ICellGetter }], SelectedAssets.Count: 0 },
                100,
                ContextActionGroup.Viewing,
                viewCommand,
                context => menuItemProvider.View(viewCommand, context),
                () => menuItemProvider.View(viewCommand).HotKey,
                true),
        ];
    }

    public IEnumerable<ContextAction> GetActions() => _actions;
}
