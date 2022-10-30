using System.Collections;
using CreationEditor.GUI.Models.Record.Browser;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.GUI.ViewModels.Record;

public interface IRecordListVM {
    public Type Type { get; set; }
    [Reactive] public IEnumerable Records { get; set; }
    [Reactive] public IRecordBrowserSettings RecordBrowserSettings { get; set; }
    
    [Reactive] public bool IsBusy { get; set; }
}
