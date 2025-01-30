using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Record;

public interface IRecordValidator {
    IEnumerable<string> Validate(IMajorRecordGetter record);
}

public sealed class DebugRecordValidator : IRecordValidator {
    public IEnumerable<string> Validate(IMajorRecordGetter record) {
        if (record.EditorID is null) {
            yield return "EditorID is null";
        } else {
            if (record.EditorID.Contains("bug")) {
                yield return "Shouldn't have bug";
            }
        }
    }
}
