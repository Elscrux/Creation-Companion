using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Actions;

public sealed class PlacedContextMenuProvider : IRecordContextMenuProvider {
    public static Type? Type => null;

    public IRecordActionsProvider RecordActionsProvider { get; }
    public ICommand PrimaryCommand => RecordActionsProvider.Edit;
    public IEnumerable<object> MenuItems { get; }

    public PlacedContextMenuProvider(
        PlacedActionsProvider recordActionsProvider,
        IMenuItemProvider menuItemProvider,
        IObservable<IPlacedGetter?> selectedPlaced) {
        RecordActionsProvider = recordActionsProvider;

        var placedBinding = selectedPlaced.ToBinding();
        MenuItems = new object[] {
            menuItemProvider.New(recordActionsProvider.New),
            menuItemProvider.Edit(recordActionsProvider.Edit, placedBinding),
            new MenuItem {
                Header = "Edit Base",
                Command = recordActionsProvider.EditBase,
                CommandParameter = placedBinding
            },
            menuItemProvider.Duplicate(recordActionsProvider.Duplicate, placedBinding),
            menuItemProvider.Delete(recordActionsProvider.Delete, placedBinding),
            new Separator(),
            menuItemProvider.References(recordActionsProvider.OpenReferences, placedBinding)
        };
    }
}
