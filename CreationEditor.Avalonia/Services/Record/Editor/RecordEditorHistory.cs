using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Models.Mod.Editor;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.Editor;

public interface IDatedItem<out T> {
    T Item { get; }
    DateTime Date { get; }
}

public record DatedItem<T>(T Item, DateTime Date) : IDatedItem<T>;

public sealed class RecordEditorHistory<TEditableRecord, TMajorRecord, TMajorRecordGetter>
    where TEditableRecord : class, IEditableRecord<TMajorRecord>, TMajorRecord
    where TMajorRecord : class, IMajorRecordInternal, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {
    private const int MaxStates = 100;
    private readonly Action<TMajorRecordGetter> _applyUpdate;
    private readonly ObservableCollectionExtended<IDatedItem<TMajorRecordGetter>> _recordStates = [];
    private readonly List<IDatedItem<TMajorRecordGetter>> _redoStates = [];

    public ReadOnlyObservableCollection<IDatedItem<TMajorRecordGetter>> RecordStates { get; }

    public RecordEditorHistory(IRecordEditorCore<TEditableRecord, TMajorRecord, TMajorRecordGetter> vm, Action<TMajorRecordGetter> applyUpdate) {
        _applyUpdate = applyUpdate;

        RecordStates = new ReadOnlyObservableCollection<IDatedItem<TMajorRecordGetter>>(_recordStates);
        Update(vm.Record);

        vm.RecordChanged
            .Where(x => x.Reason != RecordUpdateReason.History)
            .Subscribe(x => Update(x.Record))
            .DisposeWith(vm);
    }

    public void Update(TMajorRecordGetter record) {
        _redoStates.Clear();
        _recordStates.Add(new DatedItem<TMajorRecordGetter>(record, DateTime.Now));

        OnStatesUpdated();
    }

    public void SetToState(IDatedItem<TMajorRecordGetter> datedItem) {
        var indexOf = RecordStates.IndexOf(datedItem);
        if (indexOf == -1) return;

        _applyUpdate(datedItem.Item);
        _recordStates.RemoveRange(indexOf, _recordStates.Count - indexOf);
        OnStatesUpdated();
    }

    public void Undo() {
        // Skip if there is only the initial state
        if (_recordStates.Count == 1) return;

        _redoStates.Add(_recordStates[^1]);
        _recordStates.RemoveAt(_recordStates.Count - 1);

        _applyUpdate(_recordStates[^1].Item);

        OnStatesUpdated();
    }

    public void Redo() {
        if (_redoStates.Count == 0) return;

        _recordStates.Add(_redoStates[^1]);
        _redoStates.RemoveAt(_redoStates.Count - 1);

        _applyUpdate(_recordStates[^1].Item);

        OnStatesUpdated();
    }

    private void OnStatesUpdated() {
        if (_recordStates.Count > MaxStates) {
            _recordStates.RemoveAt(0);
        }
    }
}
