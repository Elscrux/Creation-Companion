using System.Diagnostics.CodeAnalysis;
using CreationEditor.Services.Mutagen.References.Record;
namespace CreationEditor.Avalonia.Services.Record.Actions;

public sealed record RecordListContext(
    IReadOnlyList<IReferencedRecord> SelectedRecords,
    IEnumerable<Type> ListTypes,
    IDictionary<Type, object>? Settings) {

    private IDictionary<Type, object>? Settings { get; } = Settings;

    public T GetSetting<T>() {
        return (T) Settings![typeof(T)];
    }

    public bool TryGetSetting<T>([MaybeNullWhen(false)] out T setting) {
        if (Settings != null && Settings.TryGetValue(typeof(T), out var obj) && obj is T t) {
            setting = t;
            return true;
        }

        setting = default;
        return false;
    }
}
