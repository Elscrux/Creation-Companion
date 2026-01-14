using CreationEditor.Avalonia.Services.Record.Prefix;
using CreationEditor.Services.Environment;
using LeveledList.Model;
using LeveledList.Model.Enchantments;
using LeveledList.Model.Feature;
using LeveledList.Services.LeveledList;
using LeveledList.Services.Record;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace LeveledList.Services.Enchantments;

public class EnchantmentsGenerator(
    IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
    ILeveledListRecordTypeProvider leveledListRecordTypeProvider,
    ITierController tierController,
    IFeatureProvider featureProvider,
    IRecordPrefixService recordPrefixService,
    EnchantmentProvider enchantmentProvider) {
    public IEnumerable<EnchantedItem> Generate(ListRecordType type, EnchantmentItem enchantment, IReadOnlyCollection<IModGetter> modsToLookAt) {
        foreach (var record in leveledListRecordTypeProvider.GetRecords(modsToLookAt, type)) {
            if (record.Record is not (IEnchantableGetter enchantable and IKeywordedGetter { Keywords: {} keywords })) continue;

            var keywordFormKeys = keywords.Select(x => x.FormKey).ToHashSet();

            // Check if the enchantable has any enchantment set - if so skip it
            var level = enchantmentProvider.GetEnchantmentLevel(enchantable);
            if (level != -1) continue;

            var recordTier = tierController.GetRecordTier(enchantable);
            if (recordTier is null) continue;

            // Get enchantments listed for the tier
            var tierData = tierController.GetTierData(type, recordTier);
            if (tierData is null) continue;
            if (tierData.Levels.Count == 0) continue;

            // Only generated enchanted items when the item tier has an enchantment for every level
            // For example iron has enchantment levels 1-3 but an enchantment that is only available for levels 3-6
            // doesn't cover the iron range properly - so it should not be generated for iron at all
            // A further sanity check is made to prevent level ranges from 1-6 being blocked entirely
            // by enchantments ini the range 3-6, which should still be included - so also check that if there are at least
            // 3 shared levels between the enchantment and the tier, it's allowed
            if (type != ListRecordType.Staff) {
                var minimumEnchantmentLevel = enchantment.Tiers.Min(x => x.Key);
                var minimumTierLevel = tierData.Levels.Min();
                if (minimumTierLevel < minimumEnchantmentLevel) {
                    var sharedLevels = tierData.Levels.Intersect(enchantment.Tiers.Keys).ToArray();
                    if (sharedLevels.Length < 3) continue;
                }
            }

            var skip = false;
            foreach (var (filteredFeatureWildcardIdentifier, allowedFeatures) in enchantment.Filter) {
                var featureWildcard = featureProvider.GetFeatureWildcard(filteredFeatureWildcardIdentifier);
                var feature = featureWildcard.Selector(enchantable)?.ToString();
                if (feature is null || allowedFeatures.Any(f => f.FeatureIdentifierMatches(feature))) continue;

                skip = true;
                break;
            }
            if (skip) continue;

            foreach (var (enchantmentLevel, enchantmentTierData) in enchantment.Tiers) {
                if (!tierData.Levels.Contains(enchantmentLevel)) continue;
                if (!editorEnvironment.LinkCache.TryResolve<IObjectEffectGetter>(enchantmentTierData.Enchantment, out var objectEffect)) continue;

                var wornRestrictions = objectEffect.WornRestrictions;
                if (objectEffect.WornRestrictions.IsNull) {
                    var baseObjectEffect = objectEffect.BaseEnchantment.TryResolve(editorEnvironment.LinkCache);
                    if (baseObjectEffect is not null) {
                        wornRestrictions = baseObjectEffect.WornRestrictions;
                    }
                }

                if (!wornRestrictions.IsNull) {
                    if (!wornRestrictions.TryResolve(editorEnvironment.LinkCache, out var keywordRestrictions)) continue;
                    if (!keywordRestrictions.Items.Any(x => keywordFormKeys.Contains(x.FormKey))) continue;
                }

                string editorId;
                if (type == ListRecordType.Staff) {
                    editorId = recordPrefixService.Prefix + "Staff" + enchantment.Suffix;
                } else {
                    editorId = recordPrefixService.Prefix + "Ench" + enchantable.EditorID?.TrimStart(recordPrefixService.Prefix, StringComparison.OrdinalIgnoreCase) + enchantment.Suffix + enchantmentLevel.ToString("D2");
                }

                editorEnvironment.LinkCache.TryResolve<IEnchantableGetter>(editorId, out var existingEnchanted);

                yield return new EnchantedItem(
                    editorId,
                    enchantmentTierData.GetFullName(enchantable),
                    enchantable,
                    objectEffect,
                    existingEnchanted,
                    enchantmentLevel,
                    recordTier);
            }
        }
    }
}
