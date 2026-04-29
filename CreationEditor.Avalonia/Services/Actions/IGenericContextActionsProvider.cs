using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Actions;

public interface IGenericContextActionsProvider {
    IMajorRecord CreateNewRecord(Type recordType);
    IMajorRecord EditRecord(IMajorRecordGetter record);
    void OpenReferences(IReferencedRecord referencedRecord, bool remap = false);
    void OpenReferences(IReferencedAsset referencedAsset, bool remap = false);
    void OpenReferences(IDataSourceLink dataSourceLink, bool remap = false);
}
