using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.DataSource;

public static class DataSourceExtensions {
    public static bool TryGetLink(this IDataSource dataSource, string fullPath, [MaybeNullWhen(false)] out DataSourceLink link) {
        if (!fullPath.StartsWith(dataSource.Path, DataRelativePath.PathComparison)) {
            link = null;
            return false;
        }

        var relativePath = dataSource.FileSystem.Path.GetRelativePath(dataSource.Path, fullPath);
        link = new DataSourceLink(dataSource, relativePath);
        return true;
    }

    public static bool TryGetDataRelativePath(this IDataSource dataSource, string fullPath, out DataRelativePath relativePath) {
        if (!fullPath.StartsWith(dataSource.Path, DataRelativePath.PathComparison)) {
            relativePath = default;
            return false;
        }

        relativePath = new DataRelativePath(dataSource.FileSystem.Path.GetRelativePath(dataSource.Path, fullPath));
        return true;
    }
}
