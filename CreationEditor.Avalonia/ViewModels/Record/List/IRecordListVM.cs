using System.Collections;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Services.Mutagen.References.Record;
using DynamicData.Binding;
using Noggog;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public interface IRecordListVM : IDisposableDropoff {
    IEnumerable? Records { get; }
    IRecordProvider RecordProvider { get; }
    IReferencedRecord? SelectedRecord { get; set; }

    IRecordContextMenuProvider RecordContextMenuProvider { get; }

    IObservableCollection<DataGridColumn> Columns { get; }

    IObservable<bool> IsBusy { get; }
}
