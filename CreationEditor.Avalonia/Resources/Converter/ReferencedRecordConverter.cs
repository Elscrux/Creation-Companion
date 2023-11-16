using Avalonia.Data.Converters;
using CreationEditor.Services.Mutagen.References.Record;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Converter;

public static class ReferencedRecordConverter {
    public static FuncValueConverter<IReferencedRecord, IMajorRecordGetter?> ToRecord { get; }
        = new(record => record?.Record);
}
