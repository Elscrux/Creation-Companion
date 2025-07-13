using System.IO.Abstractions;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.DataSource;

public interface IDataSourceLink : IComparable<IDataSourceLink>, IEquatable<IDataSourceLink> {
    IDataSource DataSource { get; }
    DataRelativePath DataRelativePath { get; }

    string FullPath { get; }
    string Name { get; }
    string NameWithoutExtension { get; }
    IFileSystem FileSystem { get; }

    DataSourceDirectoryLink? ParentDirectory { get; }

    bool Exists();

    int CompareToDirectoriesFirst(IDataSourceLink? other);
}
