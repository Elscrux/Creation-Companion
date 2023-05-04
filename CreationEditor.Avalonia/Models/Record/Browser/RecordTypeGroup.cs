using DynamicData.Binding;
namespace CreationEditor.Avalonia.Models.Record.Browser;

public sealed class RecordTypeGroup {
    public string GroupName { get; }
    public IObservableCollection<RecordTypeListing> RecordTypes { get; set; }

    private bool _activated;

    public RecordTypeGroup(string groupName, List<RecordTypeListing> recordTypes) {
        GroupName = groupName;
        RecordTypes = new ObservableCollectionExtended<RecordTypeListing>(recordTypes);
    }

    public void Activate() {
        if (_activated) return;

        _activated = true;
        foreach (var type in RecordTypes) {
            type.Activate();
        }
    }
}
