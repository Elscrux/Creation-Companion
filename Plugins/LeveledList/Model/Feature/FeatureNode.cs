using System.Diagnostics;
using LeveledList.Model.Tier;
namespace LeveledList.Model.Feature;

[DebuggerDisplay("{ToString()}}")]
public record FeatureNode(List<Feature> Features, IReadOnlyList<RecordWithTier> Records) {
    public List<FeatureNode> Children { get; } = [];
    public override string ToString() {
        return $"FeatureNode({Features.LastOrDefault()}, Records={Records.Count}, Children={Children.Count})";
    }
}
