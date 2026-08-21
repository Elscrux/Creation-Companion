using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Query;

public sealed record QueryResult(IMajorRecordGetter Record, object? QueriedField);
