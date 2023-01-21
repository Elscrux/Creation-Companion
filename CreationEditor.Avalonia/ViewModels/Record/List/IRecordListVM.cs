using System.Collections;
using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public interface IRecordListVM : IDisposable {
    public IEnumerable Records { get; }
    public IReferencedRecord? SelectedRecord { get; set; }
    
    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }
    
    public IList<IMenuItem> ContextMenuItems { get; }
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand { get; }
    
    [Reactive] public bool IsBusy { get; set; }
}
