using System.Collections;
using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Avalonia.Services.Actions;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public interface IRecordListVM : IDisposableDropoff {
    IEnumerable? Records { get; }
    IReferencedRecord? SelectedRecord { get; set; }

    ReactiveCommand<SelectedListContext, Unit> PrimaryCommand { get; }

    IObservableCollection<DataGridColumn> Columns { get; }

    IObservable<bool> IsBusy { get; }

    IContextMenuProvider ContextMenuProvider { get; }

    SelectedListContext GetRecordListContext(IReadOnlyList<IReferencedRecord> referencedRecords);
    IRecordListVM AddSetting<T>(T t);
}
