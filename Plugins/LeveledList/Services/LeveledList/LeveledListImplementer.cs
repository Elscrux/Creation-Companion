using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace LeveledList.Services.LeveledList;

public sealed class LeveledListImplementer(
    IEditorEnvironment editorEnvironment,
    IRecordController recordController) {

    public IReadOnlyList<LeveledItem> ImplementLeveledLists(IMod mod, IReadOnlyList<Model.List.LeveledList> leveledLists) {
        var leveledListEditorIDs = new Dictionary<string, LeveledItem>();

        foreach (var list in leveledLists) {
            if (leveledListEditorIDs.ContainsKey(list.EditorID)) continue;

            var leveledItem = AddLeveledList(list);
            leveledListEditorIDs.Add(list.EditorID, leveledItem);
        }

        return leveledListEditorIDs.Values.ToArray(); 

        LeveledItem AddLeveledList(Model.List.LeveledList leveledList) {
            LeveledItem leveledItem;
            if (editorEnvironment.LinkCache.TryResolve<ILeveledItemGetter>(leveledList.EditorID, out var leveledItemGetter)) {
                leveledItem = recordController.GetOrAddOverride<LeveledItem, ILeveledItemGetter>(leveledItemGetter, mod);
                if (leveledItem.Entries is null) {
                    leveledItem.Entries = [];
                } else {
                    leveledItem.Entries.Clear();
                }
                leveledItem.Flags = 0;
                leveledItem.ChanceNone = new Percent((100 - leveledList.Chance) / 100);
            } else {
                leveledItem = recordController.CreateRecord<LeveledItem, ILeveledItemGetter>(mod);
                leveledItem.Entries = [];
                leveledItem.ChanceNone = new Percent((100 - leveledList.Chance) / 100);
                leveledItem.EditorID = leveledList.EditorID;
            }

            if (leveledList.UseAll) {
                leveledItem.Flags |= LeveledItem.Flag.UseAll;
            } else {
                if (leveledList.CalculateForEach) leveledItem.Flags |= LeveledItem.Flag.CalculateForEachItemInCount;
                if (leveledList.CalculateFromAllLevels) leveledItem.Flags |= LeveledItem.Flag.CalculateFromAllLevelsLessThanOrEqualPlayer;
                if (leveledList.SpecialLoot) leveledItem.Flags |= LeveledItem.Flag.SpecialLoot;
            }

            if (leveledList.SpecialLoot) leveledItem.Flags |= LeveledItem.Flag.SpecialLoot;

            foreach (var entry in leveledList.Entries) {
                FormKey formKey;
                if (entry.Item.Record is null) {
                    if (leveledListEditorIDs.TryGetValue(entry.Item.List!.EditorID, out var item)) {
                        // Use the existing leveled list form key if it exists
                        formKey = item.FormKey;
                    } else {
                        // Otherwise, create a new leveled list already
                        var entryList = AddLeveledList(entry.Item.List);
                        formKey = entryList.FormKey;
                    }
                } else {
                    formKey = entry.Item.Record.FormKey;
                }

                leveledItem.Entries.Add(new LeveledItemEntry {
                    Data = new LeveledItemEntryData {
                        Count = entry.Count,
                        Level = entry.Level,
                        Reference = new FormLink<IItemGetter>(formKey),
                    },
                });
            }

            return leveledItem;
        }
    }
}
