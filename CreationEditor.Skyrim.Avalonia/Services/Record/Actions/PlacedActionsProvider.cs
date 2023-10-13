using System;
using System.Reactive;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Actions;

public sealed class PlacedActionsProvider : IRecordActionsProvider {
    private readonly RecordActionsProvider<IPlaced, IPlacedGetter> _placedActionsProvider;

    public ReactiveCommand<Type?, Unit> New => _placedActionsProvider.New;
    public ReactiveCommand<object?, Unit> Edit => _placedActionsProvider.Edit;
    public ReactiveCommand<object?, Unit> EditBase { get; }
    public ReactiveCommand<object?, Unit> Duplicate => _placedActionsProvider.Duplicate;
    public ReactiveCommand<object?, Unit> Delete => _placedActionsProvider.Delete;
    public ReactiveCommand<object?, Unit> OpenReferences => _placedActionsProvider.OpenReferences;

    public PlacedActionsProvider(
        RecordActionsProvider<IPlaced, IPlacedGetter> placedActionsProvider,
        IEditorEnvironment editorEnvironment,
        IRecordEditorController recordEditorController,
        IRecordController recordController) {
        _placedActionsProvider = placedActionsProvider;

        EditBase = ReactiveCommand.Create<object?>(obj => {
            if (obj is not IPlacedObjectGetter placedObject) return;

            var placeable = placedObject.Base.TryResolve(editorEnvironment.LinkCache);
            if (placeable is null) return;

            var newOverride = recordController.GetOrAddOverride<IPlaceableObject, IPlaceableObjectGetter>(placeable);
            recordEditorController.OpenEditor(newOverride);
        });
    }
}
