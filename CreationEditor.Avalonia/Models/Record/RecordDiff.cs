using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Models.Record;

public sealed class RecordDiff(IMajorRecordGetter old, IMajorRecordGetter @new) {
    public IMajorRecordGetter Old { get; } = old;
    public IMajorRecordGetter New { get; } = @new;
}
