using System.Collections;
using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Services.Mutagen.References.Record;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public interface IRecordListVM : IDisposableDropoff {
    IEnumerable? Records { get; }
    IReferencedRecord? SelectedRecord { get; set; }

    public ReactiveCommand<RecordListContext, Unit> PrimaryCommand { get; }

    IObservableCollection<DataGridColumn> Columns { get; }

    IObservable<bool> IsBusy { get; }

    IRecordContextMenuProvider RecordContextMenuProvider { get; }

    RecordListContext GetRecordListContext(IReadOnlyList<IReferencedRecord> referencedRecords);
    IRecordListVM AddSetting<T>(T t);
}
