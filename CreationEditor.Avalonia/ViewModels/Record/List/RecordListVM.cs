using System.Reactive;
using Avalonia.Controls;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public sealed class RecordListVM<TMajorRecord, TMajorRecordGetter> : ARecordListVM<ReferencedRecord<TMajorRecord, TMajorRecordGetter>>
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {

    public ReactiveCommand<Unit, Unit> NewRecord { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DuplicateSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedRecord { get; }

    public RecordListVM(
        IReferenceQuery referenceQuery,
        IRecordListFactory recordListFactory,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IRecordEditorController recordEditorController,
        IRecordController recordController)
        : base(recordListFactory, recordBrowserSettingsVM, referenceQuery, recordController) {
        NewRecord = ReactiveCommand.Create(() => {
            var newRecord = RecordController.CreateRecord<TMajorRecord, TMajorRecordGetter>();
            recordEditorController.OpenEditor<TMajorRecord, TMajorRecordGetter>(newRecord);
            
            var referencedRecord = new ReferencedRecord<TMajorRecord, TMajorRecordGetter>(newRecord);
            RecordCache.AddOrUpdate(referencedRecord);
        });
        
        EditSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            var newOverride = RecordController.GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
            recordEditorController.OpenEditor<TMajorRecord, TMajorRecordGetter>(newOverride);
        });
        
        DuplicateSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            
            var duplicate = RecordController.DuplicateRecord<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
            
            var referencedRecord = new ReferencedRecord<TMajorRecord, TMajorRecordGetter>(duplicate);
            RecordCache.AddOrUpdate(referencedRecord);
        });
        
        DeleteSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            
            RecordController.DeleteRecord<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
            RecordCache.Remove(SelectedRecord);
        });

        this.WhenAnyValue(x => x.RecordBrowserSettingsVM.LinkCache)
            .DoOnGuiAndSwitchBack(_ => IsBusy = true)
            .Subscribe(linkCache => {
                RecordCache.Clear();
                RecordCache.Refresh();
                RecordCache.Edit(updater => {
                    foreach (var record in linkCache.PriorityOrder.WinningOverrides<TMajorRecordGetter>()) {
                        var formLinks = ReferenceQuery.GetReferences(record.FormKey, RecordBrowserSettingsVM.LinkCache);
                        var referencedRecord = new ReferencedRecord<TMajorRecord, TMajorRecordGetter>(record, formLinks);

                        updater.AddOrUpdate(referencedRecord);
                    }
                });
                
                Dispatcher.UIThread.Post(() => IsBusy = false);
            })
            .DisposeWith(this);

        recordEditorController.RecordChanged
            .Subscribe(majorRecord => {
                if (majorRecord is not TMajorRecordGetter record) return;
                if (!RecordCache.TryGetValue(record.FormKey, out var listRecord)) return;
                
                // Modify value
                listRecord.Record = record;
                
                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);
        
        ContextMenuItems.Insert(0, new MenuItem { Header = "New", Command = NewRecord });
        ContextMenuItems.Insert(1, new MenuItem { Header = "Edit", Command = EditSelectedRecord });
        ContextMenuItems.Insert(2, new MenuItem { Header = "Duplicate", Command = DuplicateSelectedRecord });
        ContextMenuItems.Insert(3, new MenuItem { Header = "Delete", Command = DeleteSelectedRecord });

        DoubleTapCommand = EditSelectedRecord;
    }
}