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
namespace LeveledList.Services.Enchantments;

public class EnchantmentsGenerator(
    IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
    ILeveledListRecordTypeProvider leveledListRecordTypeProvider,
    ITierController tierController,
    IFeatureProvider featureProvider,
    EnchantmentProvider enchantmentProvider) {
    public IEnumerable<EnchantedItem> Generate(ListRecordType type, EnchantmentItem enchantment, IModGetter modToLookAt) {
        foreach (var record in leveledListRecordTypeProvider.GetRecords(modToLookAt, type)) {
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
            if (type != ListRecordType.Staff) {
                var minimumEnchantmentLevel = enchantment.Tiers.Min(x => x.Key);
                var minimumTierLevel = tierData.Levels.Min();
                if (minimumTierLevel < minimumEnchantmentLevel) continue;
            }

            var skip = false;
            foreach (var (filteredFeatureWildcardIdentifier, allowedFeatures) in enchantment.Filter) {
                var featureWildcard = featureProvider.GetFeatureWildcard(filteredFeatureWildcardIdentifier);
                var feature = featureWildcard.Selector(enchantable)?.ToString();
                if (feature is null || allowedFeatures.Any(f => f.FeatureIdentifierEquals(feature))) continue;

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
                    editorId = "Staff" + enchantment.Suffix;
                } else {
                    editorId = "Ench" + enchantable.EditorID + enchantment.Suffix + enchantmentLevel.ToString("D2");
                }

                yield return new EnchantedItem(
                    editorId,
                    enchantmentTierData.GetFullName(enchantable),
                    enchantable,
                    objectEffect,
                    enchantmentLevel,
                    recordTier);
            }
        }
    }
}
