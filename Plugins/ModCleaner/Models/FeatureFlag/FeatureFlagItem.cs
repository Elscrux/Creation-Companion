using CreationEditor.Avalonia.Models.Selectables;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace ModCleaner.Models.FeatureFlag;

public sealed partial class FeatureFlagItem(FeatureFlag featureFlag, bool isSelected = false) : ReactiveObject, IReactiveSelectable {
    public FeatureFlagItem() : this(new FeatureFlag("", default!, [], [])) { }
    [Reactive] public partial bool IsSelected { get; set; } = isSelected;
    public FeatureFlag FeatureFlag { get; } = featureFlag;
}
