using System.Reactive;
using Avalonia.Controls;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.Provider;

public sealed class RecordTypeProvider : ViewModel, IRecordProvider<IReferencedRecord> {
    public Type Type { get; }
    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);
    [Reactive] public IReferencedRecord? SelectedRecord { get; set; }
    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }

    [Reactive] public bool IsBusy { get; set; }
    
    public IList<IMenuItem> ContextMenuItems { get; } = new List<IMenuItem>();
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand => null;
    

    public RecordTypeProvider(
        Type type,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IReferenceQuery referenceQuery,
        IRecordController recordController) {
        Type = type;
        RecordBrowserSettingsVM = recordBrowserSettingsVM;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettingsVM);

        this.WhenAnyValue(x => x.RecordBrowserSettingsVM.LinkCache)
            .DoOnGuiAndSwitchBack(_ => IsBusy = true)
            .Subscribe(linkCache => {
                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var record in linkCache.AllIdentifiers(Type)) {
                        var formLinks = referenceQuery.GetReferences(record.FormKey, RecordBrowserSettingsVM.LinkCache);
                        var referencedRecord = new ReferencedRecord<IMajorRecordIdentifier, IMajorRecordIdentifier>(record, formLinks);

                        updater.AddOrUpdate(referencedRecord);
                    }
                });
                
                Dispatcher.UIThread.Post(() => IsBusy = false);
            });

        recordController.RecordChanged
            .Subscribe(record => {
                if (record.GetType() != Type) return;
                
                if (RecordCache.TryGetValue(record.FormKey, out var listRecord)) {
                    // Modify value
                    listRecord.Record = record;
                } else {
                    // Create new entry
                    var formLinks = referenceQuery.GetReferences(record.FormKey, RecordBrowserSettingsVM.LinkCache);
                    listRecord = new ReferencedRecord<IMajorRecordIdentifier, IMajorRecordIdentifier>(record, formLinks);
                }
                
                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);
    }
}
