using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Type;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Query;

public interface IQueryFrom {
    QueryFromItem? SelectedItem { get; set; }
    IList<QueryFromItem> Items { get; }

    IEnumerable<IMajorRecordGetter> GetRecords();
}

public sealed record QueryFromItem(string Name, Type Type);

public sealed class QueryFromRecordType : ReactiveObject, IQueryFrom {
    private readonly IModScopeProvider _modScopeProvider;

    [Reactive] public QueryFromItem? SelectedItem { get; set; }
    public IList<QueryFromItem> Items { get; }

    public QueryFromRecordType(
        IModScopeProvider modScopeProvider,
        IMutagenTypeProvider mutagenTypeProvider) {
        _modScopeProvider = modScopeProvider;

        Items = mutagenTypeProvider
            .GetRecordTypes(GameRelease.SkyrimSE)
            .Select(type => new QueryFromItem(type.Name, mutagenTypeProvider.GetRecordGetterType(type)))
            .ToList();
    }

    public IEnumerable<IMajorRecordGetter> GetRecords() {
        return SelectedItem is null
            ? Array.Empty<IMajorRecordGetter>()
            : _modScopeProvider.SelectedMods.WinningOverrides(SelectedItem.Type);
    }
}
