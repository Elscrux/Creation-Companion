using System.Reactive;
using CreationEditor.Avalonia.Models.Reference;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Reference;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Record.Actions;

public sealed class UntypedRecordActionsProvider : IRecordActionsProvider {
    public ReactiveCommand<Type?, Unit> New { get; }
    public ReactiveCommand<object?, Unit> Edit { get; }
    public ReactiveCommand<object?, Unit> Duplicate { get; }
    public ReactiveCommand<object?, Unit> Delete { get; }
    public ReactiveCommand<object?, Unit> OpenReferences { get; }

    public UntypedRecordActionsProvider(
        Func<object?, IReference[], ReferenceBrowserVM> referenceBrowserFactory,
        IEditorEnvironment editorEnvironment,
        IRecordController recordController,
        IRecordReferenceController recordReferenceController,
        IRecordEditorController recordEditorController,
        MainWindow mainWindow) {

        New = ReactiveCommand.Create<Type?>(type => {
            if (type is null) return;

            var newRecord = recordController.CreateRecord(type);
            recordEditorController.OpenEditor(newRecord);
        });

        Edit = ReactiveCommand.Create<object?>(obj => {
            if (obj is not IMajorRecordGetter record) return;

            var newOverride = recordController.GetOrAddOverride(record);
            recordEditorController.OpenEditor(newOverride);
        });

        Duplicate = ReactiveCommand.Create<object?>(obj => {
            if (obj is not IMajorRecordGetter record) return;

            recordController.DuplicateRecord(record);
        });

        Delete = ReactiveCommand.Create<object?>(obj => {
            if (obj is not IMajorRecordGetter record) return;

            recordController.DeleteRecord(record);
        });

        OpenReferences = ReactiveCommand.Create<object?>(obj => {
            if (obj is not IMajorRecordGetter record) return;

            var references = recordReferenceController.GetReferences(record.FormKey)
                .Select(identifier => new RecordReference(identifier, editorEnvironment, recordReferenceController))
                .Cast<IReference>()
                .ToArray();

            var referenceBrowserVM = referenceBrowserFactory(record, references);
            var referenceWindow = new ReferenceWindow(record) {
                Content = new ReferenceBrowser(referenceBrowserVM)
            };

            referenceWindow.Show(mainWindow);
        });
    }
}
