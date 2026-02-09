using System.Text.RegularExpressions;
using CreationEditor.Avalonia.Services.Record.Prefix;
using LeveledList.Model.Feature;
using LeveledList.Services.LeveledList;
using EnchantmentProvider = LeveledList.Services.LeveledList.EnchantmentProvider;
namespace LeveledList.Model.List;

public static class ListDefinitionExtensions {
    extension(ListDefinitionIdentifier identifier) {
        public IReadOnlyList<FeatureWildcardIdentifier> GetFeatureWildcards() {
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
}

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
    public const char TierGroupSeparator = '/';

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

            var span = new Span<char>(value.ToCharArray());

            // Remove all text before final group
            var lastGroupIndex = span.LastIndexOf(TierGroupSeparator);
            span = span[(lastGroupIndex + 1)..];

            // Make sure the first character is uppercase
            if (char.IsLower(span[0])) {
                span[0] = char.ToUpper(span[0]);
            }

            // Remove - and make character after it uppercase
            var dashIndex = span.IndexOf('-');
            while (dashIndex != -1) {
                if (dashIndex + 1 < span.Length && char.IsLower(span[dashIndex + 1])) {
                    span[dashIndex + 1] = char.ToUpper(span[dashIndex + 1]);
                }
                span[dashIndex] = ' '; // Replace dash with space
                dashIndex = span.IndexOf('-');
            }

            // Remove all spaces
            span = new Span<char>(span.ToArray().Where(c => c != ' ').ToArray());

            name = name.Replace($"[{feature.Wildcard.Identifier}]", span.ToString());
        }

        if (name.Contains('[')) {
            throw new InvalidOperationException($"Not all feature definitions were replaced in the name: {name}");
        }

        return name;
    }

    public LeveledList CreateLeveledItem(
        Dictionary<TierIdentifier, TierIdentifier> tierAliases,
        FeatureNode featureNode,
        EnchantmentProvider enchantmentProvider,
        IRecordPrefixService recordPrefixService,
        Func<ListDefinitionIdentifier, List<LeveledList>> listsProvider,
        Func<string, string> editorIdSelector) {
        var editorID = recordPrefixService.Prefix + GetFullName(featureNode.Features);
        editorID = editorIdSelector(editorID);

        var leveledList = new LeveledList(featureNode.Features, editorID, [], Chance, UseAll, CalculateForEach, CalculateFromAllLevels, SpecialLoot);

        if (Tiers is not null) {
            foreach (var record in featureNode.Records) {
                // Get tier entries with matching enchantment level
                var enchantmentLevel = enchantmentProvider.GetEnchantmentLevel(record.Record);

                var tierEntries = GetTierEntries(record.Tier, tierAliases);

                var feature = featureNode.Features.Find(f => f.Wildcard.Identifier == FeatureProvider.EnchantmentLevel);
                if (int.TryParse(feature?.Key.ToString(), out var enchantmentLevelOverride)) {
                    // If there is an enchantment level wildcard feature, this overrides the enchantment level of all tier entries
                    if (enchantmentLevel != enchantmentLevelOverride) {
                        tierEntries = [];
                    }
                } else {
                    // Otherwise, filter tier entries by their defined enchantment level
                    tierEntries = tierEntries
                        .Where(t => t.EnchantmentLevel == enchantmentLevel);
                }

                // Add entries for the record
                foreach (var tierEntry in tierEntries) {
                    var entries = tierEntry.GetEntries(new LeveledListEntryItem(null, record.Record));
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

                // Create entries for lists with matching features
                var (featureWildcardIdentifier, tierDefinition) = tierDefinitionsPerFeature.First();
                foreach (var (featureIdentifier, tierEntries) in tierDefinition) {
                    foreach (var list in matchingLists) {
                        // Check if the list has the required feature
                        var feature = list.Features.FirstOrDefault(f => f.Wildcard.Identifier.FeatureIdentifierMatches(featureWildcardIdentifier, tierAliases));
                        if (feature is null) continue;

                        // Check if the feature matches the required identifier
                        if (featureIdentifier != "_" && !featureIdentifier.FeatureIdentifierEquals(feature.Key.ToString(), tierAliases)) continue;

                        // Filter tier entries by the current enchantment level when available
                        var filteredTierEntries = tierEntries;
                        var enchantmentLevelFeature = list.Features.FirstOrDefault(f => f.Wildcard.Identifier == FeatureProvider.EnchantmentLevel);
                        if (enchantmentLevelFeature is not null && int.TryParse(enchantmentLevelFeature.Key.ToString(), out var enchantmentLevel)) {
                            filteredTierEntries = filteredTierEntries
                                .Where(t => t.EnchantmentLevel == enchantmentLevel)
                                .ToList();
                        }

                        // Add entries for the list
                        foreach (var wildcardTierEntry in filteredTierEntries) {
                            var entries = wildcardTierEntry.GetEntries(new LeveledListEntryItem(list, null));
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

    public IEnumerable<ListEntryDefinition> GetTierEntries(TierIdentifier tier, Dictionary<TierIdentifier, TierIdentifier> tierAliases) {
        if (Tiers is null) return [];

        // If there is a wildcard tier, return all possible tier entries
        if (Tiers.TryGetValue("_", out var listedTier)) return listedTier;

        // Return matching tier entries
        return Tiers
            .Where(t => t.Key.FeatureIdentifierMatches(tier, tierAliases))
            .SelectMany(t => t.Value);
    }

    public bool Restricts(IReadOnlyList<Feature.Feature> features, Dictionary<TierIdentifier, TierIdentifier> tierAliases) {
        if (features.Count == 0) return true;

        return features.All(f => {
            if (Restrict?.TryGetValue(f.Wildcard.Identifier, out var featureRestrictions) is not true) {
                return true;
            }

            return featureRestrictions.Any(feature => feature.FeatureIdentifierMatches(f.Key.ToString(), tierAliases));
        });
    }
}
