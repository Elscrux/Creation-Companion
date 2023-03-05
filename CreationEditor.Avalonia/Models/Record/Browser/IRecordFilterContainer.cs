using DynamicData.Binding;
namespace CreationEditor.Avalonia.Models.Record.Browser;

public interface IRecordFilterContainer {
    public string DisplayName { get; }
    public IObservableCollection<RecordFilterListing> RecordFilters { get; }
}
