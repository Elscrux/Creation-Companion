namespace LeveledList.Model.List;

public sealed record LeveledListTreeNode(
    LeveledList? LeveledList,
    LeveledListEntry? Entry);
