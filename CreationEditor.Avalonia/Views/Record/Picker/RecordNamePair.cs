using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Views.Record.Picker;

public sealed record RecordNamePair(IMajorRecordIdentifier Record, string? Name);