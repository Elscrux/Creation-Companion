using CreationEditor.Skyrim;
using LeveledList.Model;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace LeveledList.Services;

public class FeatureProvider {
    public FeatureWildcard GetFeatureWildcard(
        FeatureWildcardIdentifier featureWildcardIdentifier,
        Func<IMajorRecordGetter, TierIdentifier?> getTier,
        ILinkCache linkCache) {
        return featureWildcardIdentifier switch {
            "Tier" => new FeatureWildcard(
                featureWildcardIdentifier,
                getTier),
            "SchoolOfMagic" => new FeatureWildcard(
                featureWildcardIdentifier,
                record => {
                    if (record is not IEnchantableGetter enchantable) return null;

                    var objectEffect = enchantable.ObjectEffect.TryResolve(linkCache);
                    return objectEffect?.GetSchoolOfMagic(linkCache);
                }),
            "MagicLevel" => new FeatureWildcard(
                featureWildcardIdentifier,
                record => {
                    if (record is not IEnchantableGetter enchantable) return null;

                    var objectEffect = enchantable.ObjectEffect.TryResolve(linkCache);
                    return objectEffect?.GetMagicLevel(linkCache);
                }),
            "Enchantment" => new FeatureWildcard(
                featureWildcardIdentifier,
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
            "WeaponType" => new FeatureWildcard(
                featureWildcardIdentifier,
                record => GetByKeyword<IWeaponGetter>(record, "WeapType", linkCache)),
            "ArmorType" => new FeatureWildcard(
                featureWildcardIdentifier,
                record => {
                    if (record is not IArmorGetter armor) return null;

                    return armor.BodyTemplate?.ArmorType switch {
                        ArmorType.Clothing => "clothing",
                        ArmorType.LightArmor => "light",
                        ArmorType.HeavyArmor => "heavy",
                        _ => null
                    };
                }),
            "ArmorSlot" => new FeatureWildcard(
                featureWildcardIdentifier,
                record => {
                    if (record is not IArmorGetter { Keywords: not null } armor) return null;

                    if (armor.Keywords.Contains(Skyrim.Keyword.ArmorBoots)) return "boots";
                    if (armor.Keywords.Contains(Skyrim.Keyword.ArmorGauntlets)) return "gauntlets";
                    if (armor.Keywords.Contains(Skyrim.Keyword.ArmorCuirass)) return "cuirass";
                    if (armor.Keywords.Contains(Skyrim.Keyword.ArmorHelmet)) return "helmet";
                    if (armor.Keywords.Contains(Skyrim.Keyword.ArmorShield)) return "shield";

                    if (armor.Keywords.Contains(Skyrim.Keyword.ClothingFeet)) return "boots";
                    if (armor.Keywords.Contains(Skyrim.Keyword.ClothingHands)) return "gauntlets";
                    if (armor.Keywords.Contains(Skyrim.Keyword.ClothingBody)) return "cuirass";
                    if (armor.Keywords.Contains(Skyrim.Keyword.ClothingHead)) return "helmet";

                    if (armor.Keywords.Contains(Skyrim.Keyword.ClothingCirclet)) return "circlet";
                    if (armor.Keywords.Contains(Skyrim.Keyword.ClothingNecklace)) return "necklace";
                    if (armor.Keywords.Contains(Skyrim.Keyword.ClothingRing)) return "ring";

                    return null;
                }),
            _ => throw new ArgumentOutOfRangeException(nameof(featureWildcardIdentifier), featureWildcardIdentifier, null)
        };
    }

    private static string? GetByKeyword<T>(IMajorRecordGetter record, string prefix, ILinkCache linkCache)
        where T : IMajorRecordGetter, IKeywordedGetter {
        if (record is not T { Keywords: not null } weapon) return null;

        var weapType = weapon.Keywords.Select(k => k.TryResolve(linkCache)?.EditorID)
            .WhereNotNull()
            .FirstOrDefault(k => k.Contains(prefix, StringComparison.OrdinalIgnoreCase));

        return weapType?.Replace(prefix, string.Empty);
    }
}
