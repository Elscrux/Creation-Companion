using LeveledList.Model.Tier;
namespace LeveledList.Model.Feature;

public record FeatureNode(List<Feature> Features, IReadOnlyList<RecordWithTier> Records) {
    public List<FeatureNode> Children { get; } = [];
}
