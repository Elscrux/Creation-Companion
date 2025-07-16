namespace LeveledList.Model.Enchantments;

public sealed record EnchantmentItem(
    string Suffix,
    Dictionary<int, EnchantmentTier> Tiers,
    Dictionary<FeatureWildcardIdentifier, List<FeatureIdentifier>> Filter) {
    public EnchantmentItem() : this(string.Empty, [], []) {}
};
