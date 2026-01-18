using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Model.Enchantments;

public sealed record EnchantedItem(
    string EditorID,
    string Name,
    IEnchantableGetter Template,
    IObjectEffectGetter Enchantment,
    IEnchantableGetter? ExistingEnchanted,
    int EnchantmentLevel,
    TierIdentifier Tier);
