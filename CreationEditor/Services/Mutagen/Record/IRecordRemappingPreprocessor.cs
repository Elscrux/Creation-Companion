using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Mutagen.Record;

/// <summary>
/// Preprocessors which will all be run before a record is remapped if applicable
/// </summary>
public interface IRecordRemappingPreprocessor {
    string Description { get; }

    bool IsApplicable(IReferencedRecord referencedRecord);
    void PreprocessRemapping(IReferencedRecord record, FormKey remappingFormKey);
}
