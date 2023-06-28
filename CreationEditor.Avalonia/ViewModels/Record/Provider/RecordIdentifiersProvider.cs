﻿using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
namespace CreationEditor.Avalonia.ViewModels.Record.Provider;

public sealed class RecordIdentifiersProvider : ViewModel, IRecordProvider<IReferencedRecord> {
    private readonly CompositeDisposable _referencesDisposable = new();

    public IEnumerable<IFormLinkIdentifier> Identifiers { get; set; }
    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);
    [Reactive] public IReferencedRecord? SelectedRecord { get; set; }
    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }

    public IObservable<bool> IsBusy { get; }

    public IList<MenuItem> ContextMenuItems { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedRecord { get; set; }
    public ReactiveCommand<Unit, Unit> DuplicateSelectedRecord { get; set; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedRecord { get; set; }
    public ReactiveCommand<Unit, Unit> DoubleTapCommand => EditSelectedRecord;

    public RecordIdentifiersProvider(
        IEnumerable<IFormLinkIdentifier> identifiers,
        IMenuItemProvider menuItemProvider,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IRecordController recordController,
        IRecordReferenceController recordReferenceController,
        IRecordEditorController recordEditorController,
        ILogger logger) {
        Identifiers = identifiers;
        RecordBrowserSettingsVM = recordBrowserSettingsVM;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettingsVM);

        EditSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord?.Record is not {} record) return;

            var newOverride = recordController.GetOrAddOverride(record);
            recordEditorController.OpenEditor(newOverride);
        });

        DuplicateSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord?.Record is not {} record) return;

            recordController.DuplicateRecord(record);
        });

        DeleteSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord?.Record is not {} record) return;

            recordController.DeleteRecord(record);
            RecordCache.Remove(SelectedRecord);
        });

        this.WhenAnyValue(
                x => x.RecordBrowserSettingsVM.LinkCache,
                x => x.Identifiers,
                (linkCache, idents) => (LinkCache: linkCache, Identifiers: idents))
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(w => w.Do(x => {
                _referencesDisposable.Clear();

                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var identifier in x.Identifiers) {
                        var formKey = identifier.FormKey;
                        if (x.LinkCache.TryResolve(formKey, identifier.Type, out var record)) {
                            recordReferenceController.GetReferencedRecord(record, out var referencedRecord).DisposeWith(_referencesDisposable);

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
            .Merge(recordController.RecordCreated)
            .Subscribe(record => {
                // Don't add if record not in the original identifiers list
                if (!RecordCache.TryGetValue(record.FormKey, out var listRecord)) return;

                // Modify value
                listRecord.Record = record;

                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);

        recordController.RecordDeleted
            .Subscribe(record => RecordCache.RemoveKey(record.FormKey))
            .DisposeWith(this);

        ContextMenuItems = new List<MenuItem> {
            menuItemProvider.Edit(EditSelectedRecord),
            menuItemProvider.Duplicate(DuplicateSelectedRecord),
            menuItemProvider.Delete(DeleteSelectedRecord),
        };
    }

    public override void Dispose() {
        base.Dispose();

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
