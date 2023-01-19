using System.Collections;
using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Models.Record.Browser;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public interface IRecordListVM : IDisposable {
    public Type Type { get; }
    public IEnumerable<IReferencedRecord> Records { get; }
    public IRecordBrowserSettings RecordBrowserSettings { get; }
    
    public IList<IMenuItem> ContextMenuItems { get; }
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand { get; }
    
    [Reactive] public bool IsBusy { get; set; }
}
