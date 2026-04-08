using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Actions;

public interface IGenericContextActionsProvider {
    IMajorRecord CreateNewRecord(Type recordType);
    IMajorRecord EditRecord(IMajorRecordGetter record);
    void OpenReferences(IReferencedRecord referencedRecord);
    void OpenReferences(IReferencedAsset referencedAsset);
    void OpenReferences(IDataSourceLink dataSourceLink);
}
