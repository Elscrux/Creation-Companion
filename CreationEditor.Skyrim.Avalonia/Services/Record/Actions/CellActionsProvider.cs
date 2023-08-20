using System;
using System.Reactive;
using System.Threading;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Actions;

public sealed class CellActionsProvider : IRecordActionsProvider {
    private readonly RecordActionsProvider<ICell, ICellGetter> _cellActionsProvider;

    public ReactiveCommand<Type?, Unit> New => _cellActionsProvider.New;
    public ReactiveCommand<object?, Unit> Edit => _cellActionsProvider.Edit;
    public ReactiveCommand<object?, Unit> Duplicate => _cellActionsProvider.Duplicate;
    public ReactiveCommand<object?, Unit> Delete => _cellActionsProvider.Delete;
    public ReactiveCommand<object?, Unit> OpenReferences => _cellActionsProvider.OpenReferences;
    public ReactiveCommand<object?, Unit> View { get; }

    public CellActionsProvider(
        RecordActionsProvider<ICell, ICellGetter> cellActionsProvider,
        IDockFactory dockFactory,
        ICellLoadStrategy cellLoadStrategy) {
        _cellActionsProvider = cellActionsProvider;

        View = ReactiveCommand.Create<object?>(obj => {
            if (obj is not ICellGetter cell) return;

            dockFactory.Open(DockElement.Viewport);

            Thread.Sleep(250); // todo remove and replace with callback

            cellLoadStrategy.LoadCell(cell);
        });
    }
}
