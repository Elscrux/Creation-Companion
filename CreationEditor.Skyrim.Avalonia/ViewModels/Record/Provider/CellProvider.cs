using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public abstract class CellProvider : ViewModel, IRecordProvider<IReferencedRecord<ICellGetter>> {
    protected readonly CompositeDisposable ReferencesDisposable = new();
    public IRecordBrowserSettings RecordBrowserSettings { get; }

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    [Reactive] public IReferencedRecord<ICellGetter>? SelectedRecord { get; set; }
    IReferencedRecord? IRecordProvider.SelectedRecord {
        get => SelectedRecord;
        set {
            if (value is IReferencedRecord<ICellGetter> referencedRecord) {
                SelectedRecord = referencedRecord;
            }
        }
    }

    public abstract IObservable<Func<IReferencedRecord, bool>> Filter { get; }

    public abstract IObservable<bool> IsBusy { get; set; }

    public IList<MenuItem> ContextMenuItems { get; }
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand { get; }

    public ReactiveCommand<Unit, Unit> ViewSelectedCell { get; }
    public ReactiveCommand<Unit, Unit> NewCell { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedCell { get; }
    public ReactiveCommand<Unit, Unit> DuplicateSelectedCell { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedCell { get; }

    protected CellProvider(ILifetimeScope lifetimeScope) {
        var newScope = lifetimeScope.BeginLifetimeScope().DisposeWith(this);
        var menuItemProvider = newScope.Resolve<IMenuItemProvider>();
        var recordController = newScope.Resolve<IRecordController>();
        var dockFactory = newScope.Resolve<IDockFactory>();
        var recordEditorController = newScope.Resolve<IRecordEditorController>();
        var recordReferenceController = newScope.Resolve<IRecordReferenceController>();

        RecordBrowserSettings = newScope.Resolve<IRecordBrowserSettings>();

        DoubleTapCommand = ViewSelectedCell = ReactiveCommand.Create(() => {
            if (SelectedRecord is null) return;

            dockFactory.Open(DockElement.Viewport);

            Thread.Sleep(250);

            LoadCell(SelectedRecord.Record);
        });

        NewCell = ReactiveCommand.Create(() => {
            var newRecord = recordController.CreateRecord<Cell, ICellGetter>();
            recordEditorController.OpenEditor<Cell, ICellGetter>(newRecord);
        });

        EditSelectedCell = ReactiveCommand.Create(() => {
            if (SelectedRecord is null) return;

            var newOverride = recordController.GetOrAddOverride<Cell, ICellGetter>(SelectedRecord.Record);
            recordEditorController.OpenEditor<Cell, ICellGetter>(newOverride);
        });

        DuplicateSelectedCell = ReactiveCommand.Create(() => {
            if (SelectedRecord is null) return;

            recordController.DuplicateRecord<Cell, ICellGetter>(SelectedRecord.Record);
        });

        DeleteSelectedCell = ReactiveCommand.Create(() => {
            if (SelectedRecord is null) return;

            recordController.DeleteRecord<Cell, ICellGetter>(SelectedRecord.Record);
            RecordCache.Remove(SelectedRecord);
        });

        recordController.RecordChanged
            .Merge(recordController.RecordCreated)
            .Subscribe(majorRecord => {
                if (majorRecord is not ICellGetter record) return;

                if (RecordCache.TryGetValue(record.FormKey, out var listRecord)) {
                    // Modify value
                    listRecord.Record = record;
                } else {
                    // Create new entry
                    recordReferenceController.GetReferencedRecord(record, out var outListRecord).DisposeWith(this);
                    listRecord = outListRecord;
                }

                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);

        recordController.RecordDeleted
            .Subscribe(record => RecordCache.RemoveKey(record.FormKey))
            .DisposeWith(this);

        ContextMenuItems = new List<MenuItem> {
            menuItemProvider.View(ViewSelectedCell),
            menuItemProvider.New(NewCell),
            menuItemProvider.Edit(EditSelectedCell),
            menuItemProvider.Duplicate(DuplicateSelectedCell),
            menuItemProvider.Delete(DeleteSelectedCell),
        };
    }

    protected abstract void LoadCell(ICellGetter cell);

    public void TrySelect(ICellGetter cell) {
        if (!RecordCache.TryGetValue(cell.FormKey, out var referencedRecord)) return;
        if (referencedRecord is not IReferencedRecord<ICellGetter> referencedCell) return;

        SelectedRecord = null;
        SelectedRecord = referencedCell;
    }

    public override void Dispose() {
        base.Dispose();

        RecordCache.Clear();
        RecordCache.Dispose();
        ReferencesDisposable.Dispose();
    }
}
