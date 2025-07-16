namespace LeveledList.Model.Enchantments;

public sealed record TierEnchantmentLevel(
    TierIdentifier Tier,
    List<int> EnchantmentLevels);
