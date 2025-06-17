using System.Text.RegularExpressions;
using CreationEditor;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace ConsoleApp1;

public class RecordTypeProvider(IModGetter mod) {
    public IEnumerable<IMajorRecordGetter> GetRecords(string type) {
        return type switch {
            "armor" => mod.EnumerateMajorRecords<IArmorGetter>(),
            "weapon" => mod.EnumerateMajorRecords<IWeaponGetter>(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}

public class FormKeyYamlTypeConverter : IYamlTypeConverter {
    public bool Accepts(Type type) => type == typeof(FormKey);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer) {
        var value = parser.Consume<YamlDotNet.Core.Events.Scalar>().Value;
        return FormKey.Factory(value);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer) {
        var formKey = (FormKey) value!;
        emitter.Emit(new YamlDotNet.Core.Events.Scalar(formKey.ToString()));
    }
}

public record CreatedList(List<Wildcard> Wildcards, List<CreatedListItem> LeveledItem);
public record CreatedListItem(Container Container, LeveledItem LeveledItem);

public class Generator {
    public void Generate() {
        var env = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);
        var modToLookAt = env.LinkCache.PriorityOrder[^1];
        var recordTypeProvider = new RecordTypeProvider(modToLookAt);
        var featureProvider = new FeatureProvider();
        var enchantmentProvider = new EnchantmentProvider(env.LinkCache);
        var mod = new SkyrimMod(ModKey.FromFileName("GeneratedLeveledLists.esp"), SkyrimRelease.SkyrimSE, 1.7f);

        var fileStream = File.Open(@"C:\Users\nickp\Downloads\leveled-list-configs\lists\ench-weapons.yaml", FileMode.Open);
        var deserializer = new DeserializerBuilder()
            .WithTypeConverter(new FormKeyYamlTypeConverter())
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build();

        var def = deserializer.Deserialize<ListDefinition>(new StreamReader(fileStream));
        var records = recordTypeProvider.GetRecords(def.Type).ToArray();

        var createdLists = new Dictionary<string, CreatedList>();

        foreach (var (listName, list) in def.Lists) {
            var wildcardStrings = list.GetWildcards().ToArray();

            var wildcards = wildcardStrings
                .Select(wildcard => featureProvider.GetFeature(wildcard, env.LinkCache, r => GetTier(r, def.Tiers)))
                .ToList();

            var rootContainer = GroupByWildcards(records, wildcards, []) ?? new Container([], records);
            var fullyGroupedLeveledLists = rootContainer.GetTreeLeaves(c => c.Children)
                .Where(c => c.Features.Count == 0 || c.Features.All(f => {
                    if (list.Restrict?.TryGetValue(f.Wildcard.Name, out var restrictions) is true) {
                        return restrictions.Contains(f.Key);
                    }
                    return true;
                }))
                .ToList();

            var createdList = new CreatedList(wildcards, []);
            createdLists.Add(listName, createdList);

            foreach (var fullyGroupedLeveledList in fullyGroupedLeveledLists) {
                var fullName = list.GetFullName(fullyGroupedLeveledList.Features);
                LeveledItem leveledItem;
                if (env.LinkCache.TryResolveIdentifier<ILeveledItemGetter>(fullName, out var formKey)) {
                    var context = env.LinkCache.ResolveContext<LeveledItem, ILeveledItemGetter>(formKey);
                    leveledItem = context.GetOrAddAsOverride(mod);
                    if (leveledItem.Entries is null) {
                        leveledItem.Entries = [];
                    } else {
                        leveledItem.Entries.Clear();
                    }
                    leveledItem.Flags = 0;
                    leveledItem.ChanceNone = new Percent((100 - list.Chance) / 100);
                } else {
                    leveledItem = new LeveledItem(mod) {
                        EditorID = fullName,
                        ChanceNone = new Percent((100 - list.Chance) / 100),
                        Entries = [],
                    };
                }
                createdList.LeveledItem.Add(new CreatedListItem(fullyGroupedLeveledList, leveledItem));

                if (list.UseAll) {
                    leveledItem.Flags |= LeveledItem.Flag.UseAll;
                } else {
                    if (list.CalculateForEach) leveledItem.Flags |= LeveledItem.Flag.CalculateForEachItemInCount;
                    if (list.CalculateFromAllLevels) leveledItem.Flags |= LeveledItem.Flag.CalculateFromAllLevelsLessThanOrEqualPlayer;
                    if (list.SpecialLoot) leveledItem.Flags |= LeveledItem.Flag.SpecialLoot;
                }

                if (list.Tiers is not null) {
                    foreach (var record in fullyGroupedLeveledList.Records) {
                        var enchantmentLevel = enchantmentProvider.GetEnchantmentLevel(record);

                        GetValue(list.Tiers, record, enchantmentLevel);
                    }
                }

                if (list.IncludeLists is not null) {
                    foreach (var (includedListName, wildcardTierDefinition) in list.IncludeLists) {
                        var includedList = createdLists[includedListName];

                        // Group by shared features
                        var matchingLists = new List<CreatedListItem>();
                        foreach (var leveledItems in includedList.LeveledItem) {
                            var allFeaturesMatching = leveledItems.Container.Features
                                .Where(f => wildcards.Contains(f.Wildcard))
                                .All(f => fullyGroupedLeveledList.Features.Contains(f));

                            if (allFeaturesMatching) {
                                matchingLists.Add(leveledItems);
                            }
                        }

                        var (wildcardName, wildcardItems) = wildcardTierDefinition.First();
                        foreach (var (wildcardKey, wildcardTiers) in wildcardItems) {
                            foreach (var (container, item) in matchingLists) {
                                var feature = container.Features.FirstOrDefault(f => f.Wildcard.Name == wildcardName);
                                if (feature is null) continue;
                                if (wildcardKey != "_" && feature.Key.ToString() != wildcardKey) continue;

                                foreach (var wildcardTier in wildcardTiers) {
                                    AddEntry(wildcardTier, item);
                                }
                            }
                        }
                    }
                }

                // if (leveledItem.Entries.Count > 0) {
                if (!mod.LeveledItems.ContainsKey(leveledItem.FormKey)) {
                    mod.LeveledItems.Add(leveledItem);
                }
                // }

                void AddEntry(TierItem tierItem, IMajorRecordGetter record) {
                    var currentLevel = tierItem.Level;
                    for (var i = 0; i < tierItem.Amount; i++) {
                        var entry = new LeveledItemEntry {
                            Data = new LeveledItemEntryData {
                                Reference = new FormLink<IItemGetter>(record.FormKey),
                                Level = currentLevel,
                                Count = tierItem.Count,
                            }
                        };
                        leveledItem.Entries?.Add(entry);
                        currentLevel += tierItem.Interval;
                    }
                }

                void GetValue(Dictionary<string, List<TierItem>>? listedTiers, IMajorRecordGetter record, int enchantmentLevel) {
                    var tier = GetTier(record, def.Tiers);
                    if (tier is null) return;

                    if (listedTiers is null) return;

                    List<TierItem>? tiers;
                    if (listedTiers.Keys.Contains("_")) {
                        tiers = listedTiers["_"];
                    } else {
                        if (listedTiers.TryGetValue(tier, out tiers) is not true) return;
                    }

                    foreach (var listTierItem in tiers) {
                        if (listTierItem.EnchantmentLevel != enchantmentLevel) continue;

                        AddEntry(listTierItem, record);
                    }
                }
            }
        }

        mod.WriteToBinary(@"C:\Users\nickp\beyond-skyrim\modlist\overwrite\" + mod.ModKey.FileName);
    }

    public string? GetTier(IMajorRecordGetter record, List<string> tiers) {
        var recordEditorId = record.EditorID;
        if (recordEditorId is null) return null;

        return tiers.FirstOrDefault(tier => recordEditorId.Contains(tier, StringComparison.OrdinalIgnoreCase));
    }

    public Container? GroupByWildcards(
        IEnumerable<IMajorRecordGetter> records,
        List<Wildcard> wildcards,
        List<Feature> features,
        int level = 0) {
        var recordList = records.ToList();
        var container = new Container(features, recordList);
        if (level >= wildcards.Count) return container;

        var wildcard = wildcards[level];
        var groups = recordList.GroupBy(r => wildcard.Selector(r));
        foreach (var group in groups) {
            if (group.Key is null) continue;

            var feature = new Feature(wildcard, group.Key);
            var newFeatures = features.Append(feature).ToList();
            var child = GroupByWildcards(group, wildcards, newFeatures, level + 1);
            if (child is null) continue;

            container.Children.Add(child);
        }

        if (container.Children.Count == 0) {
            return null;
        }

        return container;
    }
}

public partial class EnchantmentProvider(ILinkCache linkCache) {
    [GeneratedRegex(@"\d+$")]
    public static partial Regex EnchantmentRegex { get; }

    public int GetEnchantmentLevel(IMajorRecordGetter record) {
        switch (record) {
            case IEnchantableGetter enchantable:
                if (enchantable.ObjectEffect.IsNull) return 0;

                var enchantment = enchantable.ObjectEffect.TryResolve(linkCache);
                if (enchantment is null) return 0;

                var editorId = enchantment.EditorID;
                if (editorId is null) return 0;

                var match = EnchantmentRegex.Match(editorId);
                if (!match.Success) return 0;

                if (!int.TryParse(match.Value, out var level)) return 0;

                return level;
            default:
                return 0;
        }
    }
}

public record Container(List<Feature> Features, IReadOnlyList<IMajorRecordGetter> Records) {
    public List<Container> Children { get; } = [];
}

public class FeatureProvider {
    public Wildcard GetFeature(string wildcard, ILinkCache linkCache, Func<IMajorRecordGetter, string?> getTier) {
        return wildcard switch {
            "Tier" => new Wildcard(
                wildcard,
                record => getTier(record)),
            "SchoolOfMagic" => new Wildcard(
                wildcard,
                record => {
                    if (record is not IEnchantableGetter enchantable) return null;

                    var objectEffect = enchantable.ObjectEffect.TryResolve(linkCache);
                    return objectEffect?.GetSchoolOfMagic(linkCache);
                }),
            "MagicLevel" => new Wildcard(
                wildcard,
                record => {
                    if (record is not IEnchantableGetter enchantable) return null;

                    var objectEffect = enchantable.ObjectEffect.TryResolve(linkCache);
                    return objectEffect?.GetMagicLevel(linkCache);
                }),
            "Enchantment" => new Wildcard(
                wildcard,
                record => {
                    if (record is not IEnchantableGetter enchantable) return null;

                    var enchantment = enchantable.ObjectEffect.TryResolve(linkCache);
                    if (enchantment is null) return null;

                    while (!enchantment.BaseEnchantment.IsNull) {
                        if (enchantment.FormKey == enchantment.BaseEnchantment.FormKey) break;

                        enchantment = enchantment.BaseEnchantment.TryResolve(linkCache);
                        if (enchantment is null) return null;
                    }

                    return enchantment.EditorID?
                        .Replace("Ench", string.Empty)
                        .Replace("Weapon", string.Empty)
                        .Replace("Armor", string.Empty)
                        .Replace("Staff", string.Empty)
                        .Replace("Damage", string.Empty)
                        .Replace("Base", string.Empty);
                }),
            "WeaponType" => new Wildcard(
                wildcard,
                record => {
                    if (record is not IWeaponGetter { Keywords: not null } weapon) return null;

                    const string weaponTypeStr = "WeapType";
                    var weapType = weapon.Keywords
                        .Select(k => k.TryResolve(linkCache)?.EditorID)
                        .WhereNotNull()
                        .FirstOrDefault(k => {
                            var index = k.IndexOf(weaponTypeStr, StringComparison.OrdinalIgnoreCase);
                            return index != -1;
                        });

                    return weapType?
                        .Replace(weaponTypeStr, string.Empty);
                }),
            _ => throw new ArgumentOutOfRangeException(nameof(wildcard), wildcard, null)
        };
    }
}

public static class ObjectEffectExtensions {
    public static ActorValue? GetSchoolOfMagic(this IObjectEffectGetter objectEffect, ILinkCache linkCache) {
        foreach (var effect in objectEffect.Effects) {
            var magicEffect = effect.BaseEffect.TryResolve(linkCache);
            if (magicEffect is null) continue;

            return magicEffect.MagicSkill;
        }

        return null;
    }

    public static uint GetMagicLevel(this IObjectEffectGetter objectEffect, ILinkCache linkCache) {
        var max = objectEffect.Effects
            .Select(e => e.BaseEffect.TryResolve(linkCache))
            .WhereNotNull()
            .Select(e => e.MinimumSkillLevel)
            .Max();

        return max;
    }
}

public record Wildcard(string Name, Func<IMajorRecordGetter, object?> Selector) {
    public virtual bool Equals(Wildcard? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name == other.Name;
    }
    public override int GetHashCode() => Name.GetHashCode();

}

public record Feature(Wildcard Wildcard, object Key);

public record ListDefinition(
    string Type,
    List<string> Tiers,
    Dictionary<string, ListItem> Lists,
    Dictionary<string, Item>? Items = null
) {
    public ListDefinition() : this(string.Empty, [], []) {}
}

public record Item(FormKey FormKey, string Tier = "") {
    public Item() : this(FormKey.Null) {}
}

public record TierItem(
    short Level,
    short Count = 1,
    int Amount = 1,
    short Interval = 0,
    int EnchantmentLevel = 0
) {
    public TierItem() : this(1) {}
}

public partial record ListItem(
    string Suffix,
    Dictionary<string, List<string>>? Restrict = null,
    Dictionary<string, List<TierItem>>? Tiers = null,
    Dictionary<string, Dictionary<string, Dictionary<string, List<TierItem>>>>? IncludeLists = null,
    float Chance = 100.0f,
    bool UseAll = false,
    bool CalculateForEach = true,
    bool CalculateFromAllLevels = true,
    bool SpecialLoot = false) {


    public ListItem() : this(string.Empty) {}

    [GeneratedRegex(@"\[([^\]]+)\]")]
    public static partial Regex WildcardRegex { get; }

    public IEnumerable<string> GetWildcards() {
        var matches = WildcardRegex.Matches(Suffix);
        foreach (Match match in matches) {
            yield return match.Groups[1].Value;
        }
    }

    public string GetFullName(IReadOnlyList<Feature> features) {
        if (features.Count == 0) return Suffix;

        var name = Suffix;
        foreach (var feature in features) {
            var value = feature.Key.ToString();
            if (value is null) continue;

            // Make sure the first character is uppercase
            if (char.IsLower(value, 0)) {
                value = char.ToUpper(value[0]) + value[1..];
            }

            name = name.Replace($"[{feature.Wildcard.Name}]", value);
        }

        if (name.Contains("[")) {
            throw new InvalidOperationException($"Not all wildcards were replaced in the name: {name}");
        }

        return name;
    }
}
