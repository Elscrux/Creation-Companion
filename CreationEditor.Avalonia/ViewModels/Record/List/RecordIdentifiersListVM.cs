using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public sealed class RecordIdentifiersListVM : ARecordListVM<IReferencedRecordIdentifier> {
    public RecordIdentifiersListVM(
        IEnumerable<IFormLinkIdentifier> identifiers,
        IRecordListFactory recordListFactory,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IReferenceQuery referenceQuery, 
        IRecordController recordController)
        : base(recordListFactory, recordBrowserSettingsVM, referenceQuery, recordController) {

        RecordCache.Clear();
        RecordCache.Edit(updater => {
            foreach (var identifier in identifiers) {
                if (recordBrowserSettingsVM.LinkCache.TryResolve(identifier.FormKey, identifier.Type, out var record)) {
                    var formLinks = ReferenceQuery.GetReferences(record.FormKey, RecordBrowserSettingsVM.LinkCache);
                    var referencedRecord = new ReferencedRecord<IMajorRecordIdentifier, IMajorRecordIdentifier>(record, formLinks);

                    updater.AddOrUpdate(referencedRecord);
                }
            }
        });
    }
}
