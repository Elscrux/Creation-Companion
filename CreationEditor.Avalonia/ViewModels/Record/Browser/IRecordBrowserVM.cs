using System.Reactive;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Filter;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public interface IRecordBrowserVM : IDisposableDropoff {
    IObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }

    ReactiveCommand<RecordTypeGroup, Unit> SelectRecordTypeGroupCommand { get; }
    ReactiveCommand<RecordTypeListing, Unit> SelectRecordTypeCommand { get; }
    ReactiveCommand<RecordFilterListing, Unit> SelectRecordFilterCommand { get; }

    IRecordListVM? RecordListVM { get; set; }
    IRecordBrowserSettings RecordBrowserSettings { get; }
}
