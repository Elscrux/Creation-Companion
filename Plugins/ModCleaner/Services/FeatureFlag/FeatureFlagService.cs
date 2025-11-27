using System.Reactive;
using System.Reactive.Subjects;
using CreationEditor.Services.State;
namespace ModCleaner.Services.FeatureFlag;

public sealed class FeatureFlagService : IFeatureFlagService {
    private readonly Dictionary<Models.FeatureFlag.FeatureFlag, bool> _featureFlags = [];

    public IStateRepository<Models.FeatureFlag.FeatureFlag, Models.FeatureFlag.FeatureFlag, string> StateRepository { get; }


    public IObservable<Unit> FeatureFlagsChanged => _featureFlagsChanged;
    private readonly BehaviorSubject<Unit> _featureFlagsChanged = new(Unit.Default);

    public IReadOnlyDictionary<Models.FeatureFlag.FeatureFlag, bool> FeatureFlags => _featureFlags;

    public IEnumerable<Models.FeatureFlag.FeatureFlag> EnabledFeatureFlags => FeatureFlags.Where(kv => kv.Value).Select(kv => kv.Key);

    public FeatureFlagService(
        IStateRepositoryFactory<Models.FeatureFlag.FeatureFlag, Models.FeatureFlag.FeatureFlag, string> stateRepositoryFactory) {
        StateRepository = stateRepositoryFactory.CreateCached("FeatureFlags");

        foreach (var featureFlag in StateRepository.LoadAll()) {
            _featureFlags.Add(featureFlag, true);
        }

        _featureFlagsChanged.OnNext(Unit.Default);
    }

    public bool IsFeatureEnabled(Models.FeatureFlag.FeatureFlag featureFlag) {
        return FeatureFlags.GetValueOrDefault(featureFlag, false);
    }

    public void SetFeatureEnabled(Models.FeatureFlag.FeatureFlag featureFlag, bool enabled) {
        var previouslyEnabled = IsFeatureEnabled(featureFlag);
        _featureFlags[featureFlag] = enabled;

        if (previouslyEnabled != enabled) _featureFlagsChanged.OnNext(Unit.Default);
    }

    public void AddFeatureFlag(Models.FeatureFlag.FeatureFlag featureFlag, bool enabled = true) {
        _featureFlags[featureFlag] = enabled;
        _featureFlagsChanged.OnNext(Unit.Default);
        StateRepository.Update(_ => featureFlag, featureFlag.Name);
    }

    public void RemoveFeatureFlag(Models.FeatureFlag.FeatureFlag featureFlag) {
        if (!_featureFlags.Remove(featureFlag)) return;

        StateRepository.Delete(featureFlag.Name);
        _featureFlagsChanged.OnNext(Unit.Default);
    }
}
