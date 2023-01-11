using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Models.Record.Browser;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public interface IRecordListVM : IDisposable {
    public Type Type { get; }
    public IEnumerable<IReferencedRecord> Records { get; }
    public IRecordBrowserSettings RecordBrowserSettings { get; }
    
    [Reactive] public bool IsBusy { get; set; }
}
