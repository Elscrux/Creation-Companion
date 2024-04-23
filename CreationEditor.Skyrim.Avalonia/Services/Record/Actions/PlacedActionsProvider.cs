using System.Collections.Generic;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Actions;

public sealed class PlacedActionsProvider : IRecordActionsProvider {
    private readonly IList<RecordAction> _actions;

    public PlacedActionsProvider(
        IMenuItemProvider menuItemProvider,
        ILinkCacheProvider linkCacheProvider,
        IRecordEditorController recordEditorController,
        IRecordController recordController) {

        var editBaseCommand = ReactiveCommand.Create<RecordListContext>(context => {
            if (context.SelectedRecords[0].Record is not IPlacedObjectGetter placedObject) return;

            var placeable = placedObject.Base.TryResolve(linkCacheProvider.LinkCache);
            if (placeable is null) return;

            var newOverride = recordController.GetOrAddOverride<IPlaceableObject, IPlaceableObjectGetter>(placeable);
            recordEditorController.OpenEditor(newOverride);
        });

        _actions = [
            new RecordAction(context => context.SelectedRecords is [{ Record: IPlacedObjectGetter }],
                45,
                RecordActionGroup.Modification,
                editBaseCommand,
                context => menuItemProvider.View(editBaseCommand, context),
                () => menuItemProvider.View(editBaseCommand).HotKey)
        ];

    }

    public IEnumerable<RecordAction> GetActions() => _actions;
}
