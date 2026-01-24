using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace LeveledList.Model.Tier;

public sealed partial class TierItem : ReactiveObject {
    [Reactive] public partial TierIdentifier Identifier { get; set; } = string.Empty;
    public string EnchantmentLevels { get; set; } = string.Empty;

    public List<int> GetEnchantmentLevels() {
        return EnchantmentLevels
            .Split(',')
            .Select(x => x.Trim())
            .Select(x => int.TryParse(x, out var level) ? level : -1)
            .Where(x => x != -1)
            .ToList();
    }

    public override string ToString() => Identifier;
}
