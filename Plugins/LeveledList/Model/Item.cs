using Mutagen.Bethesda.Plugins;
namespace LeveledList.Model;

public sealed record Item(FormKey FormKey, TierIdentifier Tier = "") {
    public Item() : this(FormKey.Null) {}
}
