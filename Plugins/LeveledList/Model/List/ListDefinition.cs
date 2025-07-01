using System.Text.RegularExpressions;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using EnchantmentProvider = LeveledList.Services.LeveledList.EnchantmentProvider;
namespace LeveledList.Model.List;

public partial record ListDefinition(
    ListDefinitionIdentifier Name,
    Dictionary<FeatureWildcardIdentifier, List<FeatureIdentifier>>? Restrict = null,
    Dictionary<TierIdentifier, List<ListEntryDefinition>>? Tiers = null,
    Dictionary<ListDefinitionIdentifier, Dictionary<FeatureWildcardIdentifier, Dictionary<FeatureIdentifier, List<ListEntryDefinition>>>>? IncludeLists = null,
    float Chance = 100.0f,
    bool UseAll = false,
    bool CalculateForEach = true,
    bool CalculateFromAllLevels = true,
    bool SpecialLoot = false) {

    public ListDefinition() : this(string.Empty) {}

    [GeneratedRegex(@"\[([^\]]+)\]")]
    public static partial Regex FeatureWildcardRegex { get; }

    public IEnumerable<FeatureWildcardIdentifier> GetFeatureWildcardIdentifiers() {
        var matches = FeatureWildcardRegex.Matches(Name);
        foreach (Match match in matches) {
            yield return match.Groups[1].Value;
        }
    }

    public string GetFullName(IReadOnlyList<Feature.Feature> features) {
        if (features.Count == 0) return Name;

        var name = Name;
        foreach (var feature in features) {
            var value = feature.Key.ToString();
            if (value is null) continue;

            // Make sure the first character is uppercase
            if (char.IsLower(value, 0)) {
                value = char.ToUpper(value[0]) + value[1..];
            }

            name = name.Replace($"[{feature.Wildcard.Identifier}]", value);
        }

        if (name.Contains('[')) {
            throw new InvalidOperationException($"Not all feature definitions were replaced in the name: {name}");
        }

        return name;
    }

    public LeveledItem CreateLeveledItem(
        List<Feature.Feature> features,
        IReadOnlyList<IMajorRecordGetter> records,
        EnchantmentProvider enchantmentProvider,
        ITierController tierController,
        Func<ListDefinitionIdentifier, List<CreatedLeveledList>> listsProvider,
        ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache,
        ISkyrimMod mod) {
        var fullName = GetFullName(features);
        LeveledItem leveledItem;
        if (linkCache.TryResolveIdentifier<ILeveledItemGetter>(fullName, out var formKey)) {
            var context = linkCache.ResolveContext<LeveledItem, ILeveledItemGetter>(formKey);
            leveledItem = context.GetOrAddAsOverride(mod);
            if (leveledItem.Entries is null) {
                leveledItem.Entries = [];
            } else {
                leveledItem.Entries.Clear();
            }
            leveledItem.Flags = 0;
            leveledItem.ChanceNone = new Percent((100 - Chance) / 100);
        } else {
            leveledItem = new LeveledItem(mod) {
                EditorID = fullName,
                ChanceNone = new Percent((100 - Chance) / 100),
                Entries = [],
            };
        }

        if (UseAll) {
            leveledItem.Flags |= LeveledItem.Flag.UseAll;
        } else {
            if (CalculateForEach) leveledItem.Flags |= LeveledItem.Flag.CalculateForEachItemInCount;
            if (CalculateFromAllLevels) leveledItem.Flags |= LeveledItem.Flag.CalculateFromAllLevelsLessThanOrEqualPlayer;
            if (SpecialLoot) leveledItem.Flags |= LeveledItem.Flag.SpecialLoot;
        }

        if (Tiers is not null) {
            foreach (var record in records) {
                var enchantmentLevel = enchantmentProvider.GetEnchantmentLevel(record);

                var tierIdentifier = tierController.GetRecordTier(record);
                if (tierIdentifier is null) continue;

                var tiers = GetTiers(tierIdentifier)
                    .Where(t => t.EnchantmentLevel == enchantmentLevel);

                foreach (var listTierItem in tiers) {
                    var entries = listTierItem.GetEntries(record);
                    leveledItem.Entries!.AddRange(entries);
                }
            }
        }

        if (IncludeLists is not null) {
            foreach (var (listDefinitionIdentifier, tierDefinitionsPerFeature) in IncludeLists) {
                var lists = listsProvider(listDefinitionIdentifier);

                // Group by shared features
                var wildcards = features.Select(x => x.Wildcard.Identifier).ToHashSet();
                var matchingLists = lists
                    .Where(leveledList => leveledList.FeatureNode.Features
                        .Where(f => wildcards.Contains(f.Wildcard.Identifier))
                        .All(features.Contains))
                    .ToArray();

                var (featureWildcardIdentifier, tierDefinition) = tierDefinitionsPerFeature.First();
                foreach (var (featureIdentifier, tiers) in tierDefinition) {
                    foreach (var (container, item) in matchingLists) {
                        var feature = container.Features.FirstOrDefault(f => f.Wildcard.Identifier == featureWildcardIdentifier);
                        if (feature is null) continue;
                        if (featureIdentifier != "_" && feature.Key.ToString() != featureIdentifier) continue;

                        foreach (var wildcardTier in tiers) {
                            var entries = wildcardTier.GetEntries(item);
                            leveledItem.Entries!.AddRange(entries);
                        }
                    }
                }
            }
        }

        return leveledItem;
    }

    public IEnumerable<ListEntryDefinition> GetTiers(TierIdentifier tier) {
        if (Tiers is null) return [];

        // If there is a wildcard tier, return all possible tiers
        if (Tiers.TryGetValue("_", out var listedTier)) return listedTier;

        // If the tier is found, return the specific tiers for that tier
        if (Tiers.TryGetValue(tier, out var tiers)) return tiers;

        // Otherwise, no tiers are available for this definition
        return [];
    }
}
