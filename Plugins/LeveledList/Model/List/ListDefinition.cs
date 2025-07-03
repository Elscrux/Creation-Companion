using System.Text.RegularExpressions;
using LeveledList.Model.Feature;
using EnchantmentProvider = LeveledList.Services.LeveledList.EnchantmentProvider;
namespace LeveledList.Model.List;

public static class ListDefinitionExtensions {
    public static IReadOnlyList<FeatureWildcardIdentifier> GetFeatureWildcards(this ListDefinitionIdentifier identifier) {
        var wildcards = new List<FeatureWildcardIdentifier>();
        var readOnlySpan = identifier.AsSpan();

        var startIndex = identifier.AsSpan().IndexOf('[');
        while (startIndex != -1) {
            var endIndex = readOnlySpan.IndexOf(']');
            if (endIndex < startIndex) break;

            var wildcard = readOnlySpan[(startIndex + 1)..endIndex];
            wildcards.Add(new FeatureWildcardIdentifier(wildcard.ToString()));

            readOnlySpan = readOnlySpan[(endIndex + 1)..];
            startIndex = readOnlySpan.IndexOf('[');
        }

        return wildcards;
    }
}

public partial record ListDefinition(
    ListDefinitionIdentifier Name,
    Dictionary<FeatureWildcardIdentifier, List<FeatureIdentifier>>? Restrict = null,
    Dictionary<TierIdentifier, List<ListEntryDefinition>>? Tiers = null,
    Dictionary<ListDefinitionIdentifier, Dictionary<FeatureWildcardIdentifier, Dictionary<FeatureIdentifier, List<ListEntryDefinition>>>>?
        IncludeLists = null,
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

    public LeveledList CreateLeveledItem(
        FeatureNode featureNode,
        EnchantmentProvider enchantmentProvider,
        Func<ListDefinitionIdentifier, List<LeveledList>> listsProvider) {
        var fullName = GetFullName(featureNode.Features);
        var leveledList = new LeveledList(featureNode.Features, fullName, [], Chance, UseAll, CalculateForEach, CalculateFromAllLevels, SpecialLoot);

        if (Tiers is not null) {
            foreach (var record in featureNode.Records) {
                var enchantmentLevel = enchantmentProvider.GetEnchantmentLevel(record.Record);

                var tiers = GetTiers(record.Tier)
                    .Where(t => t.EnchantmentLevel == enchantmentLevel);

                foreach (var listTierItem in tiers) {
                    var entries = listTierItem.GetEntries(new LeveledListEntryItem(null, record.Record));
                    leveledList.Entries.AddRange(entries);
                }
            }
        }

        if (IncludeLists is not null) {
            foreach (var (listDefinitionIdentifier, tierDefinitionsPerFeature) in IncludeLists) {
                var lists = listsProvider(listDefinitionIdentifier);

                // Group by shared features
                var wildcards = featureNode.Features.Select(x => x.Wildcard.Identifier).ToHashSet();
                var matchingLists = lists
                    .Where(list => list.Features
                        .Where(f => wildcards.Contains(f.Wildcard.Identifier))
                        .All(featureNode.Features.Contains))
                    .ToArray();

                var (featureWildcardIdentifier, tierDefinition) = tierDefinitionsPerFeature.First();
                foreach (var (featureIdentifier, tiers) in tierDefinition) {
                    foreach (var list in matchingLists) {
                        var feature = list.Features.FirstOrDefault(f => f.Wildcard.Identifier == featureWildcardIdentifier);
                        if (feature is null) continue;
                        if (featureIdentifier != "_" && feature.Key.ToString() != featureIdentifier) continue;

                        foreach (var wildcardTier in tiers) {
                            var entries = wildcardTier.GetEntries(new LeveledListEntryItem(list, null));
                            leveledList.Entries.AddRange(entries);
                        }
                    }
                }
            }
        }

        // Sort the entries by level
        leveledList.Entries.Sort((a, b) => a.Level.CompareTo(b.Level));

        return leveledList;
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

    public bool Restricts(IReadOnlyList<Feature.Feature> features) {
        if (features.Count == 0) return true;

        return features.All(f => {
            if (Restrict?.TryGetValue(f.Wildcard.Identifier, out var featureRestrictions) is not true) {
                return true;
            }

            return featureRestrictions.Any(feature => StringComparer.OrdinalIgnoreCase.Equals(feature, f.Key));
        });
    }
}
