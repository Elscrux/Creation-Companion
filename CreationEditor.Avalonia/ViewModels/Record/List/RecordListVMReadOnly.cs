using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Environment;
using CreationEditor.Extension;
using DynamicData;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public sealed class RecordListVMReadOnly : ARecordListVM {
    public override Type Type { get; }

    public RecordListVMReadOnly(
        Type type,
        IRecordBrowserSettings recordBrowserSettings,
        IReferenceQuery referenceQuery, 
        IRecordController recordController)
        : base(recordBrowserSettings, referenceQuery, recordController) {
        Type = type;

        this.WhenAnyValue(x => x.RecordBrowserSettings.LinkCache)
            .DoOnGuiAndSwitchBack(_ => IsBusy = true)
            .Subscribe(linkCache => {
                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var record in linkCache.AllIdentifiers(Type)) {
                        var formLinks = ReferenceQuery.GetReferences(record.FormKey, RecordBrowserSettings.LinkCache);
                        var referencedRecord = new ReferencedRecord<IMajorRecordIdentifier, IMajorRecordIdentifier>(record, formLinks);

                        updater.AddOrUpdate(referencedRecord);
                    }
                });
                
                Dispatcher.UIThread.Post(() => IsBusy = false);
            });
    }
}
