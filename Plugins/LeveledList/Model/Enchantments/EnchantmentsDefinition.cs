namespace LeveledList.Model.Enchantments;

public sealed record EnchantmentsDefinition(
    ListRecordType Type,
    Dictionary<string, EnchantmentItem> Enchantments) {
    public EnchantmentsDefinition() : this(ListRecordType.Armor, []) {}
};
