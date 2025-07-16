using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Model.Enchantments;

public sealed record EnchantmentTier(
    FormKey Enchantment,
    string Prefix,
    string Suffix) {
    public EnchantmentTier() : this(FormKey.Null, string.Empty, string.Empty) {}
    public string GetFullName(IMajorRecordGetter record) {
        if (record is not INamedGetter { Name: {} name }) return string.Empty;

        if (string.IsNullOrEmpty(Prefix)) {
            if (string.IsNullOrEmpty(Suffix)) return name;

            return $"{name} {Suffix}";
        }

        if (string.IsNullOrEmpty(Suffix)) {
            return $"{Prefix} {name}";
        }

        return $"{Prefix} {name} {Suffix}";
    }
};
