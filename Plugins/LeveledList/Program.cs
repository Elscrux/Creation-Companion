using Autofac;
using CreationEditor.Avalonia.Modules;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Skyrim.Avalonia.Modules;
using LeveledList;
using LeveledList.Model.List;
using LeveledList.Resources;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Autofac;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var builder = new ContainerBuilder();

builder.RegisterModule<MutagenModule>();
builder.RegisterModule<EditorModule>();
builder.RegisterModule<SkyrimModule>();
builder.RegisterModule<LeveledListModule>();

var container = builder.Build();

var editorEnvironment = container.Resolve<IEditorEnvironment<ISkyrimMod, ISkyrimModGetter>>();
var recordController = container.Resolve<IRecordController>();

var modToLookAt = editorEnvironment.LinkCache.PriorityOrder[^1];
var mod = new SkyrimMod(ModKey.FromFileName("GeneratedLeveledLists.esp"), SkyrimRelease.SkyrimSE, 1.7f);

var fileStream = File.Open(@"E:\dev\leveled-list-configs\lists\base-armor-light.yaml", FileMode.Open);
var deserializer = new DeserializerBuilder()
    .WithTypeConverter(new FormKeyYamlTypeConverter())
    .WithNamingConvention(HyphenatedNamingConvention.Instance)
    .Build();

var listTypeDefinition = deserializer.Deserialize<ListTypeDefinition>(new StreamReader(fileStream));
var generator = container.Resolve<LeveledListGenerator>();
var items = generator.Generate(listTypeDefinition, editorEnvironment.LinkCache.ListedOrder[0]);

var group = mod.GetTopLevelGroup(typeof(ILeveledItemGetter));

var leveledListEditorIDs = new Dictionary<string, FormKey>();

foreach (var item in items) {
    if (!leveledListEditorIDs.ContainsKey(item.EditorID)) continue;

    var list = AddLeveledList(item);
    leveledListEditorIDs.Add(item.EditorID, list.FormKey);
}

mod.WriteToBinary(@"E:\TES\Skyrim\modlists\beyond-skyrim\overwrite\" + modToLookAt.ModKey.FileName);

Console.WriteLine("Generation done!");

LeveledItem AddLeveledList(LeveledList.Model.List.LeveledList leveledList) {
    LeveledItem leveledItem;
    if (editorEnvironment.LinkCache.TryResolve<ILeveledItemGetter>(leveledList.EditorID, out var leveledItemGetter)) {
        leveledItem = recordController.GetOrAddOverride<LeveledItem, ILeveledItemGetter>(leveledItemGetter);
        if (leveledItem.Entries is null) {
            leveledItem.Entries = [];
        } else {
            leveledItem.Entries.Clear();
        }
        leveledItem.Flags = 0;
        leveledItem.ChanceNone = new Percent((100 - leveledList.Chance) / 100);
    } else {
        leveledItem = new LeveledItem(mod.GetNextFormKey(), mod.GameRelease.ToSkyrimRelease()) {
            EditorID = leveledList.EditorID,
            ChanceNone = new Percent((100 - leveledList.Chance) / 100),
            Entries = [],
        };
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
            if (leveledListEditorIDs.TryGetValue(entry.Item.List!.EditorID, out formKey)) {
                // Use the existing leveled list form key if it exists
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

    if (!group.ContainsKey(leveledItem.FormKey)) {
        group.AddUntyped(leveledItem);
    }

    return leveledItem;
}
