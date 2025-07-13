using System.Diagnostics.CodeAnalysis;
using CreationEditor.Services.Mutagen.References;
namespace CreationEditor.Avalonia.Services.Actions;

public sealed record SelectedListContext(
    IReadOnlyList<IReferencedRecord> SelectedRecords,
    IReadOnlyList<IReferencedAsset> SelectedAssets,
    IEnumerable<Type> ListTypes,
    IDictionary<Type, object>? Settings = null) {

    public int Count => SelectedRecords.Count + SelectedAssets.Count;

    public SelectedListContext(
        IReadOnlyList<IReferencedRecord> selectedRecords,
        IReadOnlyList<IReferencedAsset> selectedAssets,
        IDictionary<Type, object>? settings = null)
        : this(
            selectedRecords,
            selectedAssets,
            selectedRecords.Select(r => r.Record.Type)
                .Concat(selectedAssets.Select(a => a.AssetLink.Type.GetType()))
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
