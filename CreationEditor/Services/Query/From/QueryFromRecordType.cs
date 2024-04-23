using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Query.Select;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Query.From;

public sealed class QueryFromRecordType : ReactiveObject, IQueryFrom {
    private readonly IModScopeProvider _modScopeProvider;

    [Reactive] public QueryFromItem? SelectedItem { get; set; }
    public IList<QueryFromItem> Items { get; }

    public QueryFromRecordType(
        IModScopeProvider modScopeProvider,
        IMutagenTypeProvider mutagenTypeProvider) {
        _modScopeProvider = modScopeProvider;

        Items = mutagenTypeProvider
            .GetRegistrations(GameRelease.SkyrimSE)
            .Select(registration => new QueryFromItem(registration.Name, registration.GetterType))
            .ToList();
    }

    public IEnumerable<IMajorRecordGetter> GetRecords() {
        return SelectedItem is null
            ? []
            : _modScopeProvider.SelectedMods.WinningOverrides(SelectedItem.Type);
    }

    public QueryFromMemento CreateMemento() {
        return new QueryFromMemento(SelectedItem is null ? null : new QueryFieldMemento(SelectedItem.Name));
    }

    public void RestoreMemento(QueryFromMemento memento) {
        SelectedItem = memento.SelectedItem is null
            ? null
            : Items.FirstOrDefault(fromItem => fromItem.Name == memento.SelectedItem.Name);
    }
}
