using System.Reactive.Linq;
using CreationEditor.Services.State;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Record.Controller;

public sealed class RecordDecorationController : IRecordDecorationController {
    private readonly IStateRepositoryFactory<object, FormKey> _stateRepositoryFactory;
    private readonly Dictionary<string, IStateRepository<object, FormKey>> _decorationStateRepos = new();
    private readonly Dictionary<FormKey, SourceCache<object, string>> _decorations = [];

    public RecordDecorationController(
        IStateRepositoryFactory<object, FormKey> stateRepositoryFactory) {
        _stateRepositoryFactory = stateRepositoryFactory;

        var dummyStateRepo = GetStateRepository("-");
        foreach (var neighboringState in dummyStateRepo.GetNeighboringStates()) {
            if (_decorationStateRepos.ContainsKey(neighboringState)) continue;

            // Create a state repository for each neighboring state
            _decorationStateRepos[neighboringState] = GetStateRepository(neighboringState);
        }
    }

    public IObservable<TDecoration> GetObservable<TDecoration>(IFormKeyGetter formKeyGetter) {
        if (!_decorations.TryGetValue(formKeyGetter.FormKey, out var decoration)) {
            decoration = CreateNewDecorations(formKeyGetter);
        }

        return decoration
            .WatchValue(GetDecorationKey<TDecoration>())
            .OfType<TDecoration>();
    }

    public IEnumerable<object> GetAll(IFormKeyGetter formKeyGetter) {
        if (_decorations.TryGetValue(formKeyGetter.FormKey, out var decoration)) return decoration.Items;

        var newDecoration = CreateNewDecorations(formKeyGetter);
        return newDecoration.Items;
    }

    public TDecoration? Get<TDecoration>(IFormKeyGetter formKeyGetter)
        where TDecoration : class {
        if (!_decorationStateRepos.TryGetValue(GetDecorationKey<TDecoration>(), out var repo)) return null;

        var value = repo.Load(formKeyGetter.FormKey);
        return value as TDecoration;
    }

    public void Update<TDecoration>(IFormKeyGetter formKeyGetter, TDecoration value)
        where TDecoration : class {
        var name = GetDecorationKey<TDecoration>();
        var repo = _decorationStateRepos.UpdateOrAdd(name, _ => GetStateRepository(name));

        var formKey = formKeyGetter.FormKey;
        _decorations.GetOrAdd(formKey, _ => CreateNewDecorations(formKey)).AddOrUpdate(value);
        repo.Save(value, formKey);
    }

    private SourceCache<object, string> CreateNewDecorations(IFormKeyGetter formKeyGetter) {
        var items = _decorationStateRepos.Values
            .Select(repo => repo.Load(formKeyGetter.FormKey))
            .WhereNotNull()
            .ToArray();

        var newDecoration = new SourceCache<object, string>(GetDecorationKey);
        _decorations[formKeyGetter.FormKey] = newDecoration;
        newDecoration.Edit(x => x.Load(items));
        return newDecoration;
    }

    private IStateRepository<object, FormKey> GetStateRepository(string stateId) {
        return _stateRepositoryFactory.Create("CustomRecordData", stateId);
    }

    private static string GetDecorationKey(object value) => value.GetType().Name;
    private static string GetDecorationKey<TDecoration>() => typeof(TDecoration).Name;
}
