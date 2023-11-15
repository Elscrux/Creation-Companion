using DynamicData.Binding;
namespace CreationEditor.Avalonia.Models.Record.Browser;

public sealed class RecordTypeGroup(string groupName, List<RecordTypeListing> recordTypes) {
    public string GroupName { get; } = groupName;
    public IObservableCollection<RecordTypeListing> RecordTypes { get; set; } = new ObservableCollectionExtended<RecordTypeListing>(recordTypes);

    private bool _activated;

    public void Activate() {
        if (_activated) return;

        _activated = true;
        foreach (var type in RecordTypes) {
            type.Activate();
        }
    }
}
