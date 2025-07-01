using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Model.Feature;

public record FeatureNode(List<Feature> Features, IReadOnlyList<IMajorRecordGetter> Records) {
    public List<FeatureNode> Children { get; } = [];
}
