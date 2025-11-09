using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Model.List;

public sealed record LeveledListTreeNode(
    LeveledList? LeveledList,
    IMajorRecordGetter? ExistingLeveledItem,
    LeveledListEntry? Entry);
