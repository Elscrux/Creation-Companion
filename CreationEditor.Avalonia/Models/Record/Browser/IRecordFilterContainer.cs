using DynamicData.Binding;
namespace CreationEditor.Avalonia.Models.Record.Browser;

public interface IRecordFilterContainer {
    string DisplayName { get; }
    IObservableCollection<RecordFilterListing> RecordFilters { get; }
}
