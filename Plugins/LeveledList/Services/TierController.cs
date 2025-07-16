using CreationEditor.Services.Mutagen.Decorations;
using CreationEditor.Services.State;
using LeveledList.Model;
using LeveledList.Model.Tier;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Services;

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
        var tier = _recordDecorationController.Get<Tier>(record.ToFormLinkInformation());
        return tier?.TierIdentifier;
    }

    public void SetRecordTier(IMajorRecordGetter record, TierIdentifier tier) {
        _recordDecorationController.Update(record.ToFormLinkInformation(), new Tier(tier));
    }

    public void RemoveRecordTier(IMajorRecordGetter record) {
        _recordDecorationController.Delete<Tier>(record);
    }

    public IReadOnlyDictionary<IFormLinkGetter, Tier> GetAllRecordTiers() {
        return _recordDecorationController
            .GetAllDecorations<Tier>();
    }

    public Dictionary<TierIdentifier, TierData> GetTiers(ListRecordType type) {
        var state = _stateRepository.Load(GetTypeKey(type));
        return state?.Tiers ?? new Dictionary<TierIdentifier, TierData>();
    }

    public TierData? GetTierData(ListRecordType type, TierIdentifier tier) {
        var state = _stateRepository.Load(GetTypeKey(type));
        return state?.Tiers.GetValueOrDefault(tier);
    }

    public void SetTiers(ListRecordType type, Dictionary<TierIdentifier, TierData> tiers) {
        _stateRepository.Save(new TierDefinitions(tiers), GetTypeKey(type));
    }

    private static string GetTypeKey(ListRecordType type) => type.ToString();
}
