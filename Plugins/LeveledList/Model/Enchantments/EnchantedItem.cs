using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Model.Enchantments;

public sealed record EnchantedItem(
    string EditorID,
    string Name,
    IEnchantableGetter Enchantable,
    IObjectEffectGetter Enchantment,
    IEnchantableGetter? ExistingEnchanted,
    int EnchantmentLevel,
    TierIdentifier Tier);
