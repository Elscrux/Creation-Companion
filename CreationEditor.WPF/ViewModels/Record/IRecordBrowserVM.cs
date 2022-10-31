using System.Collections.ObjectModel;
using System.Reactive;
using CreationEditor.WPF.Models.Record.Browser;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.ViewModels.Record;

public interface IRecordBrowserVM {
    public ObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }
    [Reactive] public IRecordListVM RecordList { get; set; }
    public ReactiveCommand<RecordTypeListing, Unit> SelectRecordType { get; }
    public IRecordBrowserSettings RecordBrowserSettings { get; }
}
