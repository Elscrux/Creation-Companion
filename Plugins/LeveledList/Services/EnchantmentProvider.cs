using System.Text.RegularExpressions;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Services;

public partial class EnchantmentProvider(ILinkCache linkCache) {
    [GeneratedRegex(@"\d+$")]
    public static partial Regex EnchantmentRegex { get; }

    public int GetEnchantmentLevel(IMajorRecordGetter record) {
        switch (record) {
            case IEnchantableGetter enchantable:
                if (enchantable.ObjectEffect.IsNull) return 0;

                var enchantment = enchantable.ObjectEffect.TryResolve(linkCache);
                if (enchantment is null) return 0;

                var editorId = enchantment.EditorID;
                if (editorId is null) return -1;

                var match = EnchantmentRegex.Match(editorId);
                if (!match.Success) return -1;

                if (!int.TryParse(match.Value, out var level)) return -1;

                return level;
            default:
                return 0;
        }
    }
}
