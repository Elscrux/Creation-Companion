using System.IO.Abstractions;
using CreationEditor.Core;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.DataSource;

/// <summary>
/// Represents a data source that acts as a data folder for the game.
/// </summary>
public interface IDataSource : IMementoProvider<DataSourceMemento> {
    string Name { get; }
    string Path { get; }
    bool IsReadOnly { get; }

    IFileSystem FileSystem { get; }

    string GetFullPath(DataRelativePath path);
    bool FileExists(DataRelativePath path);
    bool DirectoryExists(DataRelativePath path);
    IEnumerable<DataRelativePath> EnumerateFiles(DataRelativePath path, string searchPattern = "*", bool includeSubDirectories = false);
    IEnumerable<DataRelativePath> EnumerateDirectories(DataRelativePath path, string searchPattern = "*", bool includeSubDirectories = false);
}
