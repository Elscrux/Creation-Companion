using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace LeveledList.Model.Tier;

public sealed partial class TierAliasItem : ReactiveObject {
    [Reactive] public partial TierItem? Alias { get; set; }
    [Reactive] public partial TierItem? Original { get; set; }
}
