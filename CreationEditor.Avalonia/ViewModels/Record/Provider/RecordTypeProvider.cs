using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Services.Mutagen.References.Controller;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.Provider;

public sealed class RecordTypeProvider : ViewModel, IRecordProvider<IReferencedRecord> {
    public IList<Type> Types { get; }
    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);
    [Reactive] public IReferencedRecord? SelectedRecord { get; set; }
    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }

    public IObservable<bool> IsBusy { get; set; }

    public IList<IMenuItem> ContextMenuItems { get; } = new List<IMenuItem>();
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand => null;

    public RecordTypeProvider(
        IEnumerable<Type> types,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IRecordController recordController,
        IReferenceController referenceController) {
        Types = types.ToList();
        RecordBrowserSettingsVM = recordBrowserSettingsVM;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettingsVM);

        var cacheDisposable = new CompositeDisposable();

        this.WhenAnyValue(x => x.RecordBrowserSettingsVM.LinkCache)
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(x => x.Do(linkCache => {
                cacheDisposable.Clear();

                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var record in linkCache.AllIdentifiers(Types)) {
                        cacheDisposable.Add(referenceController.GetRecord(record, out var referencedRecord));

                        updater.AddOrUpdate(referencedRecord);
                    }
                });
            }), out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;

        recordController.RecordChanged
            .Subscribe(record => {
                if (!Types.Contains(record.GetType())) return;

                if (RecordCache.TryGetValue(record.FormKey, out var listRecord)) {
                    // Modify value
                    listRecord.Record = record;
                } else {
                    // Create new entry
                    referenceController.GetRecord(record, out var outListRecord);
                    listRecord = outListRecord;
                }

                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);
    }
}
