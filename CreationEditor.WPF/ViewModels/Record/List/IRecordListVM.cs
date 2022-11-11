using System.Collections;
using System.Windows.Controls;
using CreationEditor.WPF.Models.Record.Browser;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.ViewModels.Record;

public interface IRecordListVM {
    public Type Type { get; set; }
    [Reactive] public IEnumerable Records { get; set; }
    [Reactive] public IRecordBrowserSettings RecordBrowserSettings { get; set; }
    
    [Reactive] public bool IsBusy { get; set; }

    public void AddColumn(DataGridColumn column);
}
