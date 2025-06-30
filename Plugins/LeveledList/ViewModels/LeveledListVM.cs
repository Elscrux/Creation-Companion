using System.Reactive.Linq;
using CreationEditor;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Core;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.State;
using DynamicData;
using DynamicData.Binding;
using LeveledList.Model;
using LeveledList.Services;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace LeveledList.ViewModels;

public sealed partial class LeveledListRecord : ReactiveObject, IMementoProvider<LeveledListRecordMemento> {
    public required IReferencedRecord Record { get; init; }
    [Reactive] public partial TierIdentifier? Tier { get; set; }
    public Dictionary<FeatureWildcardIdentifier, object?> Features { get; init; } = [];

    public LeveledListRecordMemento CreateMemento() {
        return new LeveledListRecordMemento(Tier);
    }
}

public sealed record LeveledListRecordMemento(
    TierIdentifier? Tier);

public sealed class LeveledListVM : ViewModel {
    private readonly ITierController _tierController;
    private readonly IFeatureProvider _featureProvider;
    private readonly IStateRepository<LeveledListRecordMemento, FormKey> _stateRepository;

    private readonly IReadOnlyDictionary<FormKey, LeveledListRecordMemento> _savedMementos;
    private readonly Dictionary<Type, IReadOnlyList<FeatureWildcard>> _featureWildcards = [];

    public IObservableCollection<LeveledListRecord> Records { get; }

    public IObservable<bool> IsBusy { get; }

    public LeveledListVM(
        IStateRepositoryFactory<LeveledListRecordMemento, FormKey> stateRepositoryFactory,
        IFeatureProvider featureProvider,
        IRecordProvider recordProvider,
        ITierController tierController) {
        _featureProvider = featureProvider;
        _tierController = tierController;
        _stateRepository = stateRepositoryFactory.Create("LeveledList");
        _savedMementos = _stateRepository.LoadAllWithIdentifier();

        Records = recordProvider.RecordCache
            .Connect()
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .WrapInInProgressMarker(
                x => x.Filter(recordProvider.Filter, false),
                out var isFiltering)
            .Transform(GetLeveledListRecord)
            .ToObservableCollection(this);

        IsBusy = isFiltering
            .CombineLatest(
                recordProvider.IsBusy,
                (filtering, busy) => filtering || busy)
            .ObserveOnGui();
    }

    protected override void Dispose(bool disposing) {
        if (disposing) SaveMementos();

        base.Dispose(disposing);
    }

    private void SaveMementos() {
        foreach (var record in Records.ToArray()) {
            var memento = record.CreateMemento();
            if (memento.Tier is null) {
                _stateRepository.Delete(record.Record.FormKey);
            } else {
                _stateRepository.Save(memento, record.Record.FormKey);
            }
        }
    }

    private LeveledListRecord GetLeveledListRecord(IReferencedRecord record) {
        var features = GetFeatures(record.Record);

        var tier = _tierController.GetTier(record.Record);
        if (tier is null && _savedMementos.TryGetValue(record.FormKey, out var savedMemento)) {
            tier = savedMemento.Tier;
            if (tier is not null) {
                _tierController.SetTier(record.Record, tier);
            }
        }

        return new LeveledListRecord {
            Record = record,
            Tier = tier,
            Features = features,
        };
    }

    private Dictionary<FeatureWildcardIdentifier, object?> GetFeatures(IMajorRecordGetter record) {
        var type = record.GetType();
        if (!_featureWildcards.TryGetValue(type, out var wildcards)) {
            var featuresWildcards = _featureProvider.GetApplicableFeatureWildcards(type);
            wildcards = featuresWildcards.Select(id => _featureProvider.GetFeatureWildcard(id)).ToArray();
            _featureWildcards.Add(type, wildcards);
        }

        return wildcards.ToDictionary(w => w.Identifier, w => w.Selector(record));
    }
}
