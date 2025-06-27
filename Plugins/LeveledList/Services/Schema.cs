using CreationEditor;
using LeveledList.Model;
using LeveledList.Resources;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace LeveledList.Services;

public class Generator {
    public void Generate() {
        var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);
        var modToLookAt = env.LinkCache.PriorityOrder[^1];
        var recordTypeProvider = new RecordTypeProvider(modToLookAt);
        var featureProvider = new FeatureProvider();
        var enchantmentProvider = new EnchantmentProvider(env.LinkCache);
        var mod = new SkyrimMod(ModKey.FromFileName("GeneratedLeveledLists.esp"), SkyrimRelease.SkyrimSE, 1.7f);

        var fileStream = File.Open(@"E:\dev\leveled-list-configs\lists\base-armor-light.yaml", FileMode.Open);
        var deserializer = new DeserializerBuilder()
            .WithTypeConverter(new FormKeyYamlTypeConverter())
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build();

        var listTypeDefinition = deserializer.Deserialize<ListTypeDefinition>(new StreamReader(fileStream));
        var records = recordTypeProvider.GetRecords(listTypeDefinition.Type).ToArray();

        var createdListsPerDefinition = new Dictionary<ListDefinitionIdentifier, List<CreatedLeveledList>>();

        foreach (var (listName, listDefinition) in listTypeDefinition.Lists) {
            var featureWildcardIdentifiers = listDefinition.GetFeatureWildcardIdentifiers().ToArray();

            var featureWildcards = featureWildcardIdentifiers
                .Select(featureWildcardIdentifier => featureProvider.GetFeatureWildcard(
                    featureWildcardIdentifier,
                    r => listTypeDefinition.GetTier(r),
                    env.LinkCache))
                .ToList();

            var rootFeatureNode = GroupByFeatureWildcard(listDefinition, records, featureWildcards, []) ?? new FeatureNode([], records);
            var fullyFeaturedNodes = rootFeatureNode.GetTreeLeaves(c => c.Children)
                .Where(c => c.Features.Count == 0 || c.Features.All(f => {
                    if (listDefinition.Restrict?.TryGetValue(f.Wildcard.Identifier, out var featureRestrictions) is not true) {
                        return true;
                    }

                    return featureRestrictions.Contains(f.Key);
                }))
                .ToList();

            var createdLists = new List<CreatedLeveledList>();
            createdListsPerDefinition.Add(listName, createdLists);

            foreach (var featureNode in fullyFeaturedNodes) {
                var leveledItem = listDefinition.CreateLeveledItem(
                    featureNode.Features,
                    featureNode.Records,
                    enchantmentProvider,
                    listTypeDefinition,
                    identifier => createdListsPerDefinition[identifier],
                    env.LinkCache,
                    mod);

                createdLists.Add(new CreatedLeveledList(featureNode, leveledItem));

                if (!mod.LeveledItems.ContainsKey(leveledItem.FormKey)) {
                    mod.LeveledItems.Add(leveledItem);
                }
            }
        }

        mod.WriteToBinary(@"E:\TES\Skyrim\modlists\beyond-skyrim\overwrite\" + mod.ModKey.FileName);
    }

    public FeatureNode? GroupByFeatureWildcard(
        Model.ListDefinition listDefinition,
        IEnumerable<IMajorRecordGetter> records,
        List<FeatureWildcard> featureWildcards,
        List<Feature> features,
        int level = 0) {
        var recordList = records.ToList();
        var featureNode = new FeatureNode(features, recordList);
        if (level >= featureWildcards.Count) return featureNode;

        var featureWildcard = featureWildcards[level];
        var restrictions = listDefinition.Restrict?.GetValueOrDefault(featureWildcard.Identifier);
        var groups = recordList.GroupBy(r => featureWildcard.Selector(r));
        foreach (var group in groups) {
            if (group.Key is null) continue;
            if (restrictions is not null && !restrictions.Contains(group.Key)) continue;

            var feature = new Feature(featureWildcard, group.Key);
            var newFeatures = features.Append(feature).ToList();
            var nestedNode = GroupByFeatureWildcard(listDefinition, group, featureWildcards, newFeatures, level: level + 1);
            if (nestedNode is null) continue;

            featureNode.Children.Add(nestedNode);
        }

        if (featureNode.Children.Count == 0) {
            return null;
        }

        return featureNode;
    }
}
