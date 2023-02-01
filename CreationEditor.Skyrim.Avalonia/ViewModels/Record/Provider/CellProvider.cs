﻿using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider; 

public abstract class CellProvider : ViewModel, IRecordProvider<ReferencedRecord<Cell, ICellGetter>> {
    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }
    
    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);
    
    [Reactive] public ReferencedRecord<Cell, ICellGetter>? SelectedRecord { get; set; }
    IReferencedRecord? IRecordProvider.SelectedRecord {
        get => SelectedRecord;
        set {
            if (value is ReferencedRecord<Cell, ICellGetter> referencedRecord) {
                SelectedRecord = referencedRecord;
            }
        }
    }
    
    public abstract IObservable<Func<IReferencedRecord, bool>> Filter { get; }

    [Reactive] public bool IsBusy { get; set; }
    
    public IList<IMenuItem> ContextMenuItems { get; }
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ViewSelectedCell { get; }
    public ReactiveCommand<Unit, Unit> NewCell { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedCell { get; }
    public ReactiveCommand<Unit, Unit> DuplicateSelectedCell { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedCell { get; }

    public CellProvider(
        IRecordController recordController,
        IDockFactory dockFactory,
        IRecordEditorController recordEditorController,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IReferenceQuery referenceQuery) {
        RecordBrowserSettingsVM = recordBrowserSettingsVM;

        DoubleTapCommand = ViewSelectedCell = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            
            dockFactory.Open(DockElement.Viewport);

            Thread.Sleep(250);

            LoadCell(SelectedRecord.Record);
        });
        
        NewCell = ReactiveCommand.Create(() => {
            var newRecord = recordController.CreateRecord<Cell, ICellGetter>();
            recordEditorController.OpenEditor<Cell, ICellGetter>(newRecord);
            
            var referencedRecord = new ReferencedRecord<Cell, ICellGetter>(newRecord);
            RecordCache.AddOrUpdate(referencedRecord);
        });

        EditSelectedCell = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            var newOverride = recordController.GetOrAddOverride<Cell, ICellGetter>(SelectedRecord.Record);
            recordEditorController.OpenEditor<Cell, ICellGetter>(newOverride);
        });

        DuplicateSelectedCell = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            var duplicate = recordController.DuplicateRecord<Cell, ICellGetter>(SelectedRecord.Record);

            var referencedRecord = new ReferencedRecord<Cell, ICellGetter>(duplicate);
            RecordCache.AddOrUpdate(referencedRecord);
        });
        
        DeleteSelectedCell = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            
            recordController.DeleteRecord<Cell, ICellGetter>(SelectedRecord.Record);
            RecordCache.Remove(SelectedRecord);
        });

        recordController.RecordChanged
            .Subscribe(majorRecord => {
                if (majorRecord is not ICellGetter record) return;
                
                if (RecordCache.TryGetValue(record.FormKey, out var listRecord)) {
                    // Modify value
                    listRecord.Record = record;
                } else {
                    // Create new entry
                    var formLinks = referenceQuery.GetReferences(record.FormKey, RecordBrowserSettingsVM.LinkCache);
                    listRecord = new ReferencedRecord<Cell, ICellGetter>(record, formLinks);
                }
                
                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);

        ContextMenuItems = new List<IMenuItem> {
            new MenuItem { Header = "View", Command = ViewSelectedCell },
            new MenuItem { Header = "New", Command = NewCell },
            new MenuItem { Header = "Edit", Command = EditSelectedCell },
            new MenuItem { Header = "Duplicate", Command = DuplicateSelectedCell },
            new MenuItem { Header = "Delete", Command = DeleteSelectedCell },
        };
    }

    protected abstract void LoadCell(ICellGetter cell);
}