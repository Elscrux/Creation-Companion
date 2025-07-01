using System.Reactive.Linq;
using Autofac;
using CreationEditor.Services.State;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Record.Controller;

public sealed class RecordDecorationController(ILifetimeScope lifetimeScope) : IRecordDecorationController {
    private readonly Dictionary<string, IStateRepository<object, object, FormKey>> _decorationStateRepos = new();
    private readonly Dictionary<FormKey, SourceCache<object, string>> _decorations = [];

    public void Register<TDecoration>()
        where TDecoration : class {
        var key = GetDecorationKey<TDecoration>();
        _decorationStateRepos.UpdateOrAdd(key,
            _ => {
                var factory = lifetimeScope.Resolve<IStateRepositoryFactory<TDecoration, object, FormKey>>();
                return factory.Create("CustomRecordData", key);
            });
    }

    public IReadOnlyDictionary<FormKey, TDecoration> GetAllDecorations<TDecoration>()
        where TDecoration : class {
        if (!_decorationStateRepos.TryGetValue(GetDecorationKey<TDecoration>(), out var repo)) {
            return new Dictionary<FormKey, TDecoration>();
        }

        return repo.LoadAllWithIdentifier()
            .Where(x => x.Value is TDecoration)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => (kvp.Value as TDecoration)!);
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
        if (!_decorationStateRepos.TryGetValue(GetDecorationKey<TDecoration>(), out var repo)) return;

        var formKey = formKeyGetter.FormKey;
        _decorations.GetOrAdd(formKey, _ => CreateNewDecorations(formKey)).AddOrUpdate(value);
        repo.Save(value, formKey);
    }

    public void Delete<TDecoration>(IMajorRecordGetter record)
        where TDecoration : class {
        if (!_decorationStateRepos.TryGetValue(GetDecorationKey<TDecoration>(), out var repo)) return;

        var formKey = record.FormKey;
        repo.Delete(formKey);

        if (_decorations.TryGetValue(formKey, out var decoration)) {
            decoration.Remove(GetDecorationKey<TDecoration>());
        }
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

    private static string GetDecorationKey(object value) => value.GetType().Name;
    private static string GetDecorationKey<TDecoration>() => typeof(TDecoration).Name;
}
