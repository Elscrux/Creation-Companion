using System.Reactive;
namespace ModCleaner.Services.FeatureFlag;

public interface IFeatureFlagService {
    IObservable<Unit> FeatureFlagsChanged { get; }

    IReadOnlyDictionary<Models.FeatureFlag.FeatureFlag, bool> FeatureFlags { get; }
    IEnumerable<Models.FeatureFlag.FeatureFlag> EnabledFeatureFlags { get; }

    bool IsFeatureEnabled(Models.FeatureFlag.FeatureFlag featureFlag);
    void SetFeatureEnabled(Models.FeatureFlag.FeatureFlag featureFlag, bool enabled);

    void AddFeatureFlag(Models.FeatureFlag.FeatureFlag featureFlag, bool enabled = true);
    void RemoveFeatureFlag(Models.FeatureFlag.FeatureFlag featureFlag);
}
