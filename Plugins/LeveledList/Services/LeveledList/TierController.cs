using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Services.State;
using LeveledList.Model.Tier;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Services.LeveledList;

public sealed class TierController : ITierController {
    private readonly IStateRepository<TierDefinitions, TierDefinitions, string> _stateRepository;
    private readonly IRecordDecorationController _recordDecorationController;

    public TierController(IRecordDecorationController recordDecorationController,
        IStateRepositoryFactory<TierDefinitions, TierDefinitions, string> stateRepositoryFactory) {
        _recordDecorationController = recordDecorationController;
        _recordDecorationController.Register<Tier>();
        _stateRepository = stateRepositoryFactory.CreateCached("LeveledListTiers");
    }

    public TierIdentifier? GetRecordTier(IMajorRecordGetter record) {
        var tier = _recordDecorationController.Get<Tier>(record);
        return tier?.TierIdentifier;
    }

    public void SetRecordTier(IMajorRecordGetter record, TierIdentifier tier) {
        _recordDecorationController.Update(record, new Tier(tier));
    }

    public void RemoveRecordTier(IMajorRecordGetter record) {
        _recordDecorationController.Delete<Tier>(record);
    }

    public IReadOnlyDictionary<FormKey, Tier> GetAllRecordTiers() {
        return _recordDecorationController
            .GetAllDecorations<Tier>();
    }

    public IEnumerable<TierIdentifier> GetTiers<TMajorRecordGetter>() => GetTiers(typeof(TMajorRecordGetter));
    public IEnumerable<TierIdentifier> GetTiers(Type recordType) {
        var state = _stateRepository.Load(GetTypeKey(recordType));
        return state?.Tiers ?? [];
    }

    public void SetTiers(Type recordType, IEnumerable<TierIdentifier> tiers) {
        _stateRepository.Save(new TierDefinitions(tiers.ToList()), GetTypeKey(recordType));
    }

    private static string GetTypeKey(Type recordType) => recordType.Name;
}
