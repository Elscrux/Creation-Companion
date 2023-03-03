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
using Serilog;
namespace CreationEditor.Avalonia.ViewModels.Record.Provider;

public sealed class RecordIdentifiersProvider : ViewModel, IRecordProvider<IReferencedRecord> {
    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);
    [Reactive] public IReferencedRecord? SelectedRecord { get; set; }
    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }

    public IObservable<bool> IsBusy { get; set; }

    public IList<IMenuItem> ContextMenuItems { get; } = new List<IMenuItem>();
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand => null;

    public RecordIdentifiersProvider(
        IEnumerable<IFormLinkIdentifier> identifiers,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IRecordController recordController,
        IReferenceController referenceController,
        ILogger logger) {
        RecordBrowserSettingsVM = recordBrowserSettingsVM;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettingsVM);

        var cacheDisposable = new CompositeDisposable();

        this.WhenAnyValue(x => x.RecordBrowserSettingsVM.LinkCache)
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(x => x.Do(linkCache => {
                cacheDisposable.Clear();

                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var identifier in identifiers) {
                        var formKey = identifier.FormKey;
                        if (linkCache.TryResolve(formKey, identifier.Type, out var record)) {
                            cacheDisposable.Add(referenceController.GetRecord(record, out var referencedRecord));

                            updater.AddOrUpdate(referencedRecord);
                        } else {
                            logger.Error("Couldn't load form link {FormKey} - {Type}", formKey, identifier.Type);
                        }
                    }
                });
            }), out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;

        recordController.RecordChanged
            .Subscribe(record => {
                // Don't add if record not in the original identifiers list
                if (!RecordCache.TryGetValue(record.FormKey, out var listRecord)) return;

                // Modify value
                listRecord.Record = record;

                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);
    }
}
