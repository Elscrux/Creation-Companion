using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace LeveledList.Model.Tier;

public sealed partial class TierItem : ReactiveObject {
    [Reactive] public partial TierIdentifier Identifier { get; set; } = string.Empty;
}
