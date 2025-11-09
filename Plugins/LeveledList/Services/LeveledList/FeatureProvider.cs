using CreationEditor.Services.Environment;
using CreationEditor.Skyrim;
using LeveledList.Model.Feature;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace LeveledList.Services.LeveledList;

public sealed class FeatureProvider(
    ILinkCacheProvider linkCacheProvider,
    EnchantmentProvider enchantmentProvider,
    ITierController tierController)
    : IFeatureProvider {
    public const FeatureWildcardIdentifier Tier = "Tier";
    public const FeatureWildcardIdentifier SchoolOfMagic = "SchoolOfMagic";
    public const FeatureWildcardIdentifier MagicLevel = "MagicLevel";
    public const FeatureWildcardIdentifier Enchantment = "Enchantment";
    public const FeatureWildcardIdentifier EnchantmentLevel = "EnchantmentLevel";
    public const FeatureWildcardIdentifier WeaponType = "WeaponType";
    public const FeatureWildcardIdentifier ArmorSlot = "ArmorSlot";
    public const FeatureWildcardIdentifier ArmorType = "ArmorType";
    public const FeatureWildcardIdentifier AmmunitionType = "AmmunitionType";
    public const FeatureWildcardIdentifier PotionType = "PotionType";
    public const FeatureWildcardIdentifier PoisonType = "PoisonType";

    public FeatureWildcard GetFeatureWildcard(FeatureWildcardIdentifier featureWildcardIdentifier) {
        return featureWildcardIdentifier switch {
            Tier => new FeatureWildcard(
                featureWildcardIdentifier,
                tierController.GetRecordTier),
            SchoolOfMagic => new FeatureWildcard(
                featureWildcardIdentifier,
                record => {
                    switch (record) {
                        case IWeaponGetter { Data.AnimationType: WeaponAnimationType.Staff } staff:
                            return staff.Data.Skill;
                        case IEnchantableGetter enchantable: {
                            var objectEffect = enchantable.ObjectEffect.TryResolve(linkCacheProvider.LinkCache);
                            return objectEffect?.GetSchoolOfMagic(linkCacheProvider.LinkCache);
                        }
                        case IBookGetter { Teaches: IBookSpellGetter bookSpell }: {
                            var spell = bookSpell.Spell.TryResolve(linkCacheProvider.LinkCache);
                            return spell?.GetSchoolOfMagic(linkCacheProvider.LinkCache);
                        }
                        default: return null;
                    }

                }),
            MagicLevel => new FeatureWildcard(
                featureWildcardIdentifier,
                record => {
                    switch (record) {
                        case IEnchantableGetter enchantable: {
                            var objectEffect = enchantable.ObjectEffect.TryResolve(linkCacheProvider.LinkCache);
                            return objectEffect?.GetMagicLevel(linkCacheProvider.LinkCache);
                        }
                        case IBookGetter { Teaches: IBookSpellGetter bookSpell }: {
                            var spell = bookSpell.Spell.TryResolve(linkCacheProvider.LinkCache);
                            return spell?.GetMagicLevel(linkCacheProvider.LinkCache);
                        }
                        default: return null;
                    }
                }),
            Enchantment => new FeatureWildcard(
                featureWildcardIdentifier,
                record => {
                    if (record is not IEnchantableGetter enchantable) return null;

                    var enchantment = enchantable.ObjectEffect.TryResolve(linkCacheProvider.LinkCache);
                    if (enchantment is null) return null;

                    while (!enchantment.BaseEnchantment.IsNull) {
                        if (enchantment.FormKey == enchantment.BaseEnchantment.FormKey) break;

                        enchantment = enchantment.BaseEnchantment.TryResolve(linkCacheProvider.LinkCache);
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
            EnchantmentLevel => new FeatureWildcard(
                featureWildcardIdentifier,
                record => {
                    var level = enchantmentProvider.GetEnchantmentLevel(record);
                    if (level <= 0) return null;

                    return level.ToString("D2");
                }),
            WeaponType => new FeatureWildcard(
                featureWildcardIdentifier,
                record => GetByKeyword<IWeaponGetter>(record, "WeapType", linkCacheProvider.LinkCache)),
            ArmorType => new FeatureWildcard(
                featureWildcardIdentifier,
                record => {
                    if (record is not IArmorGetter armor) return null;

                    return armor.BodyTemplate?.ArmorType switch {
                        Mutagen.Bethesda.Skyrim.ArmorType.Clothing => "clothing",
                        Mutagen.Bethesda.Skyrim.ArmorType.LightArmor => "light",
                        Mutagen.Bethesda.Skyrim.ArmorType.HeavyArmor => "heavy",
                        _ => null
                    };
                }),
            ArmorSlot => new FeatureWildcard(
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
            AmmunitionType => new FeatureWildcard(
                featureWildcardIdentifier,
                record => {
                    if (record is not IAmmunitionGetter ammunition) return null;

                    return ammunition.Flags.HasFlag(Ammunition.Flag.NonBolt)
                        ? "arrow"
                        : "bolt";
                }),
            PotionType => new FeatureWildcard(
                featureWildcardIdentifier,
                GetFirstEffectName),
            PoisonType => new FeatureWildcard(
                featureWildcardIdentifier,
                GetFirstEffectName),
            _ => throw new ArgumentOutOfRangeException(nameof(featureWildcardIdentifier), featureWildcardIdentifier, null)
        };
    }

    private string? GetFirstEffectName(IMajorRecordGetter record) {
        if (record is not IIngestibleGetter { Effects.Count: > 0 } ingestible) return null;

        var magicEffect = ingestible.Effects[0].BaseEffect.TryResolve(linkCacheProvider.LinkCache);
        if (magicEffect is not { Name.String: {} magicEffectName }) return null;

        return magicEffectName.Replace(" ", string.Empty);
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
