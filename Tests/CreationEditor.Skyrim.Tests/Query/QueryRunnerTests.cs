using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Query;
using CreationEditor.Services.Query.From;
using CreationEditor.Services.Query.Where;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Tests.Query;

public sealed class QueryRunnerTests {
    /// <summary>
    /// Minimal scope provider over in-memory mods, since the production implementation lives in the UI layer.
    /// </summary>
    private sealed class TestModScopeProvider(params IModGetter[] mods) : IModScopeProvider {
        public BrowserScope Scope { get; set; } = BrowserScope.Environment;
        public ILinkCache LinkCache { get; } = new MutableLoadOrderLinkCache(mods.Cast<IMod>().ToArray());
        public IReadOnlyList<ISelectableModKey> Mods { get; } = [];
        public IEnumerable<ModKey> SelectedModKeys { get; } = [];
        public IEnumerable<IModGetter> SelectedMods { get; } = mods;
        public IObservable<Unit> ScopeChanged { get; } = Observable.Never<Unit>();
        public IObservable<ILinkCache> LinkCacheChanged { get; } = Observable.Never<ILinkCache>();
    }

    private static (QueryRunner Runner, QueryConditionFactory ConditionFactory, SkyrimMod Mod) CreateRunner(ILinkCacheProvider linkCacheProvider) {
        var mod = new SkyrimMod(ModKey.FromName("QueryMod", ModType.Plugin), SkyrimRelease.SkyrimSE);
        foreach (var editorId in (string[]) ["armor_c", "armor_a", "armor_b"]) {
            var armor = new Armor(mod) { EditorID = editorId };
            mod.Armors.Add(armor);
        }

        var scopeProvider = new TestModScopeProvider(mod);
        var mutagenTypeProvider = new MutagenTypeProvider();
        var queryFromFactory = new QueryFromFactory(() => new QueryFromRecordType(scopeProvider, mutagenTypeProvider));
        var compareFunctionFactory = new QueryCompareFunctionFactory(linkCacheProvider);
        QueryConditionFactory conditionFactory = null!;
        conditionFactory = new QueryConditionFactory(type => new QueryCondition(conditionFactory, compareFunctionFactory, type));

        var runner = new QueryRunner(linkCacheProvider, queryFromFactory, conditionFactory);
        runner.QueryFrom.SelectedItem = runner.QueryFrom.Items.First(item => item.Type == typeof(IArmorGetter));

        return (runner, conditionFactory, mod);
    }

    [Theory] [CreationEditorAutoData]
    public void RunQuery_FiltersByCondition(ILinkCacheProvider linkCacheProvider) {
        var (runner, conditionFactory, _) = CreateRunner(linkCacheProvider);

        var condition = (QueryCondition) conditionFactory.Create(typeof(IArmorGetter));
        condition.CompareValue = "armor_b";
        runner.QueryConditions.Add(condition);

        var results = runner.RunQuery().ToList();

        var result = Assert.Single(results);
        Assert.Equal("armor_b", result.QueriedField);
    }

    [Theory] [CreationEditorAutoData]
    public void RunQuery_OrdersBySelectedField(ILinkCacheProvider linkCacheProvider) {
        var (runner, _, _) = CreateRunner(linkCacheProvider);

        var results = runner.RunQuery().ToList();

        Assert.Equal(3, results.Count);
        Assert.Equal(["armor_a", "armor_b", "armor_c"], results.Select(r => r.QueriedField));
    }

    [Theory] [CreationEditorAutoData]
    public void RunQuery_DisabledConditions_IgnoresConditions(ILinkCacheProvider linkCacheProvider) {
        var (runner, conditionFactory, _) = CreateRunner(linkCacheProvider);
        var condition = (QueryCondition) conditionFactory.Create(typeof(IArmorGetter));
        condition.CompareValue = "armor_b";
        runner.QueryConditions.Add(condition);
        runner.EnableConditions = false;

        var results = runner.RunQuery().ToList();

        Assert.Equal(3, results.Count);
    }

    [Theory] [CreationEditorAutoData]
    public void RunQuery_DisabledSelect_ReturnsRecordAsQueriedField(ILinkCacheProvider linkCacheProvider) {
        var (runner, _, _) = CreateRunner(linkCacheProvider);
        runner.EnableSelect = false;

        var results = runner.RunQuery().ToList();

        Assert.Equal(3, results.Count);
        Assert.All(results, r => Assert.IsAssignableFrom<IArmorGetter>(r.QueriedField));
    }

    [Theory] [CreationEditorAutoData]
    public void Memento_RoundTrip_PreservesQuery(ILinkCacheProvider linkCacheProvider) {
        var (runner, conditionFactory, _) = CreateRunner(linkCacheProvider);
        var condition = (QueryCondition) conditionFactory.Create(typeof(IArmorGetter));
        condition.CompareValue = "armor_b";
        runner.QueryConditions.Add(condition);
        var memento = runner.CreateMemento();

        var (restoredRunner, restoredConditionFactory, _) = CreateRunner(linkCacheProvider);
        restoredRunner.RestoreMemento(memento);

        var results = restoredRunner.RunQuery().ToList();
        var result = Assert.Single(results);
        Assert.Equal("armor_b", result.QueriedField);
        _ = restoredConditionFactory;
    }
}
