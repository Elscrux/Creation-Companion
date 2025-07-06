using System.Reactive.Linq;
using Autofac;
using CreationEditor.Services.State;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Record.Controller;

public sealed class RecordDecorationController(ILifetimeScope lifetimeScope) : IRecordDecorationController {
    private readonly Dictionary<string, IStateRepository<object, object, IFormLinkGetter>> _decorationStateRepos = new();
    private readonly Dictionary<IFormLinkGetter, SourceCache<object, string>> _decorations = [];

    public void Register<TDecoration>()
        where TDecoration : class {
        var key = GetDecorationKey<TDecoration>();
        _decorationStateRepos.UpdateOrAdd(key,
            _ => {
                var factory = lifetimeScope.Resolve<IStateRepositoryFactory<TDecoration, object, IFormLinkGetter>>();
                return factory.Create("CustomRecordData", key);
            });
    }

    public IReadOnlyDictionary<IFormLinkGetter, TDecoration> GetAllDecorations<TDecoration>()
        where TDecoration : class {
        if (!_decorationStateRepos.TryGetValue(GetDecorationKey<TDecoration>(), out var repo)) {
            return new Dictionary<IFormLinkGetter, TDecoration>();
        }

        return repo.LoadAllWithIdentifier()
            .Where(x => x.Value is TDecoration)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => (kvp.Value as TDecoration)!);
    }

    public IObservable<TDecoration> GetObservable<TDecoration>(IFormLinkGetter formLink) {
        if (!_decorations.TryGetValue(formLink, out var decoration)) {
            decoration = CreateNewDecorations(formLink);
        }

        return decoration
            .WatchValue(GetDecorationKey<TDecoration>())
            .OfType<TDecoration>();
    }

    public IEnumerable<object> GetAll(IFormLinkGetter formLink) {
        if (_decorations.TryGetValue(formLink, out var decoration)) return decoration.Items;

        var newDecoration = CreateNewDecorations(formLink);
        return newDecoration.Items;
    }

    public TDecoration? Get<TDecoration>(IFormLinkGetter formLink)
        where TDecoration : class {
        if (!_decorationStateRepos.TryGetValue(GetDecorationKey<TDecoration>(), out var repo)) return null;

        var value = repo.Load(formLink);
        return value as TDecoration;
    }

    public void Update<TDecoration>(IFormLinkGetter formLink, TDecoration value)
        where TDecoration : class {
        if (!_decorationStateRepos.TryGetValue(GetDecorationKey<TDecoration>(), out var repo)) return;

        var formKey = formLink;
        _decorations.GetOrAdd(formKey, _ => CreateNewDecorations(formKey)).AddOrUpdate(value);
        repo.Save(value, formKey);
    }

    public void Delete<TDecoration>(IMajorRecordGetter record)
        where TDecoration : class {
        if (!_decorationStateRepos.TryGetValue(GetDecorationKey<TDecoration>(), out var repo)) return;

        var formLink = record.ToFormLinkInformation();
        repo.Delete(formLink);

        if (_decorations.TryGetValue(formLink, out var decoration)) {
            decoration.Remove(GetDecorationKey<TDecoration>());
        }
    }

    private SourceCache<object, string> CreateNewDecorations(IFormLinkGetter formLink) {
        var items = _decorationStateRepos.Values
            .Select(repo => repo.Load(formLink))
            .WhereNotNull()
            .ToArray();

        var newDecoration = new SourceCache<object, string>(GetDecorationKey);
        _decorations[formLink] = newDecoration;
        newDecoration.Edit(x => x.Load(items));
        return newDecoration;
    }

    private static string GetDecorationKey(object value) => value.GetType().Name;
    private static string GetDecorationKey<TDecoration>() => typeof(TDecoration).Name;
}
