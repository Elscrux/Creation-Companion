using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Actions;

public sealed class CellContextMenuProvider : IRecordContextMenuProvider {
    public CellActionsProvider CellActionsProvider { get; }
    public IRecordActionsProvider RecordActionsProvider => CellActionsProvider;
    public ICommand PrimaryCommand => CellActionsProvider.View;
    public IEnumerable<object> MenuItems { get; }

    public CellContextMenuProvider(
        Func<ICellLoadStrategy, CellActionsProvider> cellActionsProviderFactory,
        ICellLoadStrategy loadCell,
        IMenuItemProvider menuItemProvider,
        IObservable<ICellGetter?> selectedCell) {
        var cellActionsProvider = cellActionsProviderFactory(loadCell);
        CellActionsProvider = cellActionsProvider;

        var cellBinding = selectedCell.ToBinding();
        MenuItems = new object[] {
            menuItemProvider.New(cellActionsProvider.New),
            menuItemProvider.Edit(cellActionsProvider.Edit, cellBinding),
            menuItemProvider.View(cellActionsProvider.View, cellBinding),
            menuItemProvider.Duplicate(cellActionsProvider.Duplicate, cellBinding),
            menuItemProvider.Delete(cellActionsProvider.Delete, cellBinding),
            new Separator(),
            menuItemProvider.References(cellActionsProvider.OpenReferences, cellBinding)
        };
    }
}
