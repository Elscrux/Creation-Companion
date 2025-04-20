using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.DataSource;

public static class DataSourceExtensions {
    public static bool TryGetFileSystemLink(this IDataSource dataSource, string fullPath, [MaybeNullWhen(false)] out FileSystemLink link) {
        if (!fullPath.StartsWith(dataSource.Path, DataRelativePath.PathComparison)) {
            link = null;
            return false;
        }

        var relativePath = dataSource.FileSystem.Path.GetRelativePath(dataSource.Path, fullPath);
        link = new FileSystemLink(dataSource, relativePath);
        return true;
    }
}
