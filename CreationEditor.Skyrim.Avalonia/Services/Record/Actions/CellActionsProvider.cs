using System.Collections.Generic;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Actions;

public sealed class CellActionsProvider : IRecordActionsProvider {
    private readonly IList<RecordAction> _actions;

    public CellActionsProvider(
        IMenuItemProvider menuItemProvider,
        IDockFactory dockFactory) {

        var viewCommand = ReactiveCommand.CreateFromTask<RecordListContext>(async context => {
            if (context.SelectedRecords[0].Record is not ICellGetter cell) return;

            var cellLoadStrategy = context.GetSetting<ICellLoadStrategy>();

            await dockFactory.Open(DockElement.Viewport);

            cellLoadStrategy.LoadCell(cell);
        });

        _actions = new RecordAction[] {
            new(context => context.SelectedRecords.Count == 1 && context.SelectedRecords[0].Record is ICellGetter,
                100,
                RecordActionGroup.Viewing,
                viewCommand,
                context => menuItemProvider.View(viewCommand, context),
                true)
        };

    }

    public IEnumerable<RecordAction> GetActions() => _actions;
}
