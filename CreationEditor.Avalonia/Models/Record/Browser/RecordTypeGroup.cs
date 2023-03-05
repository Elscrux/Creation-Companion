using System.Collections.ObjectModel;
namespace CreationEditor.Avalonia.Models.Record.Browser; 

public sealed class RecordTypeGroup {
    public string GroupName { get; }
    public ObservableCollection<RecordTypeListing> RecordTypes { get; set; }

    public RecordTypeGroup(string groupName, List<RecordTypeListing> recordTypes) {
        GroupName = groupName;
        RecordTypes = new ObservableCollection<RecordTypeListing>(recordTypes);
    }
}
