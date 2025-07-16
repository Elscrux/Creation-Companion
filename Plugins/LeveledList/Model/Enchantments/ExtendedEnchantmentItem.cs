namespace LeveledList.Model.Enchantments;

public sealed record ExtendedEnchantmentItem(
    string Path,
    string FileName,
    EnchantmentItem EnchantmentItem,
    EnchantmentsDefinition EnchantmentsDefinition);
