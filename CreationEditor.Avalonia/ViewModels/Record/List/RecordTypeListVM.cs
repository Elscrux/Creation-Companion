using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public sealed class RecordTypeListVM : ARecordListVM<IReferencedRecordIdentifier> {
    public Type Type { get; }

    public RecordTypeListVM(
        Type type,
        IRecordListFactory recordListFactory,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IReferenceQuery referenceQuery, 
        IRecordController recordController)
        : base(recordListFactory, recordBrowserSettingsVM, referenceQuery, recordController) {
        Type = type;

        this.WhenAnyValue(x => x.RecordBrowserSettingsVM.LinkCache)
            .DoOnGuiAndSwitchBack(_ => IsBusy = true)
            .Subscribe(linkCache => {
                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var record in linkCache.AllIdentifiers(Type)) {
                        var formLinks = ReferenceQuery.GetReferences(record.FormKey, RecordBrowserSettingsVM.LinkCache);
                        var referencedRecord = new ReferencedRecord<IMajorRecordIdentifier, IMajorRecordIdentifier>(record, formLinks);

                        updater.AddOrUpdate(referencedRecord);
                    }
                });
                
                Dispatcher.UIThread.Post(() => IsBusy = false);
            });
    }
}
