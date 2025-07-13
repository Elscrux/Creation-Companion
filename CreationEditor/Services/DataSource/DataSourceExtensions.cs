﻿using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.DataSource;

public static class DataSourceExtensions {
    public static bool TryGetFileLink(this IDataSource dataSource, string fullPath, [MaybeNullWhen(false)] out DataSourceFileLink fileLink) {
        if (!fullPath.StartsWith(dataSource.Path, DataRelativePath.PathComparison)) {
            fileLink = null;
            return false;
        }

        var relativePath = dataSource.FileSystem.Path.GetRelativePath(dataSource.Path, fullPath);
        fileLink = new DataSourceFileLink(dataSource, relativePath);
        return true;
    }

    public static bool TryGetDirectoryLink(this IDataSource dataSource, string fullPath, [MaybeNullWhen(false)] out DataSourceDirectoryLink directoryLink) {
        if (!fullPath.StartsWith(dataSource.Path, DataRelativePath.PathComparison)) {
            directoryLink = null;
            return false;
        }

        var relativePath = dataSource.FileSystem.Path.GetRelativePath(dataSource.Path, fullPath);
        directoryLink = new DataSourceDirectoryLink(dataSource, relativePath);
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
