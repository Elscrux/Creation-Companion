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

public sealed class RecordActionsProvider<TRecord, TRecordGetter> : IRecordActionsProvider
    where TRecord : class, TRecordGetter, IMajorRecord
    where TRecordGetter : class, IMajorRecordGetter {
    public ReactiveCommand<Type?, Unit> New { get; }
    public ReactiveCommand<object?, Unit> Edit { get; }
    public ReactiveCommand<object?, Unit> Duplicate { get; }
    public ReactiveCommand<object?, Unit> Delete { get; }
    public ReactiveCommand<object?, Unit> OpenReferences { get; }

    public RecordActionsProvider(
        Func<object?, IReference[], ReferenceBrowserVM> referenceBrowserFactory,
        ILinkCacheProvider linkCacheProvider,
        IRecordController recordController,
        IRecordReferenceController recordReferenceController,
        IRecordEditorController recordEditorController,
        MainWindow mainWindow) {

        New = ReactiveCommand.Create<Type?>(type => {
            var newRecord = recordController.CreateRecord<TRecord, TRecordGetter>();
            recordEditorController.OpenEditor<TRecord, TRecordGetter>(newRecord);
        });

        Edit = ReactiveCommand.Create<object?>(obj => {
            if (obj is not TRecordGetter record) return;

            var newOverride = recordController.GetOrAddOverride<TRecord, TRecordGetter>(record);
            recordEditorController.OpenEditor<TRecord, TRecordGetter>(newOverride);
        });

        Duplicate = ReactiveCommand.Create<object?>(obj => {
            if (obj is not TRecordGetter record) return;

            recordController.DuplicateRecord<TRecord, TRecordGetter>(record);
        });

        Delete = ReactiveCommand.Create<object?>(obj => {
            if (obj is not TRecordGetter record) return;

            recordController.DeleteRecord<TRecord, TRecordGetter>(record);
        });

        OpenReferences = ReactiveCommand.Create<object?>(obj => {
            if (obj is not TRecordGetter record) return;

            var references = recordReferenceController.GetReferences(record.FormKey)
                .Select(identifier => new RecordReference(identifier, linkCacheProvider, recordReferenceController))
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
