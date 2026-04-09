using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Query;

public record QueryResult(IMajorRecordGetter Record, object? QueriedField);
