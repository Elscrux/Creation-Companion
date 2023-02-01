using System.Collections;
using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public interface IRecordListVM : IDisposable {
    public IEnumerable Records { get; }
    
    public IRecordProvider RecordProvider { get; }
    
    public IList<IMenuItem> ContextMenuItems { get; }
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand { get; }
    
    [Reactive] public bool IsBusy { get; set; }
}
