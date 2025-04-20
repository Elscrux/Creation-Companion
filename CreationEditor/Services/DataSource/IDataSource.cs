using System.IO.Abstractions;
using CreationEditor.Core;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.DataSource;

// TODO analyze asset browser startup and see what makes it so slow
// TODO cut any unneeded initialization that is not necessary - try lazy or so otherwise

/// <summary>
/// Represents a data source that acts as a data folder for the game.
/// </summary>
public interface IDataSource : IMementoProvider<DataSourceMemento> {
    string Name { get; }
    string Path { get; }
    bool IsReadOnly { get; }
    // todo replace all use of file system anywhere with this
    IFileSystem FileSystem { get; }

    string GetFullPath(DataRelativePath path);
    bool FileExists(DataRelativePath path);
}
