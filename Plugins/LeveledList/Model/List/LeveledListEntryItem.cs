using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Model.List;

public sealed record LeveledListEntryItem(LeveledList? List, IMajorRecordGetter? Record) {
    public string? EditorID => Record?.EditorID ?? List?.EditorID;
}
