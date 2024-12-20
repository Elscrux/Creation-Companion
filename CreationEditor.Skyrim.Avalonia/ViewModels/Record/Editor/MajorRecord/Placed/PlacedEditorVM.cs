using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.MajorRecord;
using CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Placed;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Placed;

public sealed class PlacedEditorVM : ViewModel, IRecordEditorVM<IPlaced, IPlacedGetter> {
    private readonly IEditorEnvironment _editorEnvironment;
    public ILinkCacheProvider LinkCacheProvider => _editorEnvironment;

    public IPlaced Record { get; set; } = null!;
    IMajorRecordGetter IRecordEditorVM.Record => Record;
    // [Reactive] public EditablePlaced EditableRecord { get; set; }

    public ReactiveCommand<Unit, Unit> Save { get; }

    public PlacedEditorVM(
        IEditorEnvironment editorEnvironment,
        IRecordController recordController,
        IRecordEditorController recordEditorController) {
        _editorEnvironment = editorEnvironment;
        // EditableRecord = new EditablePlaced(new PlacedObject(FormKey.Null, SkyrimRelease.SkyrimSE), editorEnvironment);

        Save = ReactiveCommand.Create(() => {
            // recordController.RegisterUpdate(Record, () => Record.DeepCopyIn(EditableRecord));

            recordEditorController.CloseEditor(Record);
        });
    }

    public Control CreateControl(IPlaced record) {
        return new PlacedEditor();
    }
}
