using System.Reactive;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Filter;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public interface IRecordBrowserVM : IDisposableDropoff {
    public IObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }

    public ReactiveCommand<RecordTypeGroup, Unit> SelectRecordTypeGroup { get; }
    public ReactiveCommand<RecordTypeListing, Unit> SelectRecordType { get; }
    public ReactiveCommand<RecordFilterListing, Unit> SelectRecordFilter { get; }

    public IRecordListVM? RecordListVM { get; set; }
    public IRecordBrowserSettings RecordBrowserSettings { get; }
}
