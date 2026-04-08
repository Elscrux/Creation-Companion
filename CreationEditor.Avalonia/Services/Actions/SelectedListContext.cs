using System.Diagnostics.CodeAnalysis;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Plugins;
using Noggog;
namespace CreationEditor.Avalonia.Services.Actions;

public record RecordContext(ModKey? Origin, IReferencedRecord ReferencedRecord);
public record AssetContext(IDataSourceLink DataSourceLink, IReferencedAsset? ReferencedAsset);

public sealed record SelectedListContext(
    IReadOnlyList<RecordContext> SelectedRecords,
    IReadOnlyList<AssetContext> SelectedAssets,
    IEnumerable<Type> ListTypes,
    IDictionary<Type, object>? Settings = null) {

    public int Count => SelectedRecords.Count + SelectedAssets.Count;

    public SelectedListContext(
        IReadOnlyList<RecordContext> selectedRecords,
        IReadOnlyList<AssetContext> selectedAssets,
        IDictionary<Type, object>? settings = null)
        : this(
            selectedRecords,
            selectedAssets,
            selectedRecords.Select(r => r.ReferencedRecord.Record.Type)
                .Concat(selectedAssets
                    .Select(a => a.ReferencedAsset?.AssetLink.Type.GetType())
                    .WhereNotNull())
                .Distinct(),
            settings) {}

    private IDictionary<Type, object>? Settings { get; } = Settings;

    public T GetSetting<T>() {
        return (T) Settings![typeof(T)];
    }

    public bool TryGetSetting<T>([MaybeNullWhen(false)] out T setting) {
        if (Settings is not null && Settings.TryGetValue(typeof(T), out var obj) && obj is T t) {
            setting = t;
            return true;
        }

        setting = default;
        return false;
    }
}
