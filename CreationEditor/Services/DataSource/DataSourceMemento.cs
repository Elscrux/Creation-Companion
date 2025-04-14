namespace CreationEditor.Services.DataSource;

public sealed record DataSourceMemento(
    DataSourceType Type,
    string Name,
    string Path,
    bool IsReadyOnly);

public enum DataSourceType {
    FileSystem,
    Archive,
}
