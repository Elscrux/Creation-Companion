using CreationEditor;
using CreationEditor.Avalonia.Services.Record.Prefix;
using CreationEditor.Services.Environment;
using LeveledList.Model.Feature;
using LeveledList.Model.List;
using LeveledList.Model.Tier;
using LeveledList.Services.Record;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using ListDefinition = LeveledList.Model.List.ListDefinition;
namespace LeveledList.Services.LeveledList;

public class LeveledListGenerator(
    IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
    ILeveledListRecordTypeProvider leveledListRecordTypeProvider,
    IRecordPrefixService recordPrefixService,
    IFeatureProvider featureProvider) {
    public IEnumerable<Model.List.LeveledList> Generate(ListTypeDefinition listTypeDefinition, IReadOnlyCollection<IModGetter> modsToLookAt) {
        var enchantmentProvider = new EnchantmentProvider(editorEnvironment);
        var records = leveledListRecordTypeProvider.GetRecords(modsToLookAt, listTypeDefinition.Type).ToArray();

        var createdListsPerDefinition = new Dictionary<ListDefinitionIdentifier, List<Model.List.LeveledList>>();

        foreach (var (listName, listDefinition) in listTypeDefinition.Lists) {
            var featureWildcardIdentifiers = listDefinition.GetFeatureWildcardIdentifiers().ToArray();

            var featureWildcards = featureWildcardIdentifiers
                .Select(featureProvider.GetFeatureWildcard)
                .ToList();

            var rootFeatureNode = GroupByFeatureWildcard(listDefinition, records, featureWildcards, []) ?? new FeatureNode([], records);
            var fullyFeaturedNodes = rootFeatureNode.GetTreeLeaves(c => c.Children)
                .Where(node => listDefinition.Restricts(node.Features))
                .ToList();

            var createdLists = new List<Model.List.LeveledList>();
            createdListsPerDefinition.Add(listName, createdLists);

            foreach (var featureNode in fullyFeaturedNodes) {
                var leveledItem = listDefinition.CreateLeveledItem(
                    featureNode,
                    enchantmentProvider,
                    recordPrefixService,
                    identifier => createdListsPerDefinition[identifier]);

                createdLists.Add(leveledItem);

                yield return leveledItem;
            }
        }
    }

    public static FeatureNode? GroupByFeatureWildcard(
        ListDefinition listDefinition,
        IEnumerable<RecordWithTier> records,
        List<FeatureWildcard> featureWildcards,
        List<Feature> features,
        int level = 0) {
        var recordList = records.ToList();
        var featureNode = new FeatureNode(features, recordList);
        if (level >= featureWildcards.Count) return featureNode;

        var featureWildcard = featureWildcards[level];
        var restrictions = listDefinition.Restrict?.GetValueOrDefault(featureWildcard.Identifier);
        var groups = recordList.GroupBy(r => featureWildcard.Selector(r.Record));
        foreach (var group in groups) {
            if (group.Key is null) continue;
            if (restrictions is not null && !restrictions.Any(r => r.FeatureIdentifierMatches(group.Key.ToString()))) continue;

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
