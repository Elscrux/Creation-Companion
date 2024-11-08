using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Views.Record.Picker;

public sealed record RecordNamePair(IMajorRecordIdentifierGetter Record, string? Name);
