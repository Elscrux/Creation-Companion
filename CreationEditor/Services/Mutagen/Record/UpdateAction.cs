namespace CreationEditor.Services.Mutagen.Record;

public sealed record UpdateAction<T>(T Item, Action UpdateItem);