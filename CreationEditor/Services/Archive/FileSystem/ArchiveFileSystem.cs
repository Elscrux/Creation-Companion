using System.IO.Abstractions;
using System.IO.Enumeration;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Assets;
using Noggog;
using IPath = System.IO.Abstractions.IPath;
namespace CreationEditor.Services.Archive.FileSystem;

/// <summary>
/// A read-only file system that exposes the contents of a game archive as a virtual file system.
/// <br/>The archive root is mapped to the path of the archive file itself, so all file system operations can be
/// used on the archive contents without having to deal with the specifics of the archive.
/// </summary>
public sealed class ArchiveFileSystem : FileSystemBase {
    private readonly IFileSystem _realFileSystem;
    private readonly IArchiveReader _archiveReader;
    private readonly Lazy<ArchiveIndex> _index;

    public string RootPath { get; }

    private ArchiveIndex Index => _index.Value;

    public override IPath Path { get; }
    public override IFile File { get; }
    public override IDirectory Directory { get; }
    public override IFileInfoFactory FileInfo { get; }
    public override IDirectoryInfoFactory DirectoryInfo { get; }
    public override IFileStreamFactory FileStream { get; }
    public override IFileVersionInfoFactory FileVersionInfo { get; }
    public override IDriveInfoFactory DriveInfo { get; }
    public override IFileSystemWatcherFactory FileSystemWatcher { get; }

    public ArchiveFileSystem(IFileSystem realFileSystem, IArchiveReader archiveReader, string rootPath) {
        _realFileSystem = realFileSystem;
        _archiveReader = archiveReader;
        RootPath = rootPath.TrimEnd(
            realFileSystem.Path.DirectorySeparatorChar,
            realFileSystem.Path.AltDirectorySeparatorChar);
        _index = new Lazy<ArchiveIndex>(BuildIndex);

        Path = realFileSystem.Path;
        File = new ArchiveFile(this);
        Directory = new ArchiveDirectory(this);
        FileInfo = new ArchiveFileInfoFactory(this);
        DirectoryInfo = new ArchiveDirectoryInfoFactory(this);
        FileStream = new ArchiveFileStreamFactory(this);

        // Archives are read-only snapshots, so there is nothing meaningful to version, watch or drive.
        // The real file system implementations are reused to keep the file system fully functional
        // instead of throwing for these peripheral operations.
        FileVersionInfo = realFileSystem.FileVersionInfo;
        DriveInfo = realFileSystem.DriveInfo;
        FileSystemWatcher = realFileSystem.FileSystemWatcher;
    }

    /// <summary>
    /// Converts a full path inside this file system into its path relative to the archive root.
    /// </summary>
    /// <param name="fullPath">Full path inside this file system</param>
    /// <returns>Path relative to the archive root, or null if the path is not inside this file system</returns>
    public string? GetArchiveRelativePath(string fullPath) {
        if (fullPath.IsNullOrEmpty()) return null;
        if (!fullPath.StartsWith(RootPath, DataRelativePath.PathComparison)) return null;

        if (fullPath.Length == RootPath.Length) return string.Empty;

        var separator = fullPath[RootPath.Length];
        if (separator != _realFileSystem.Path.DirectorySeparatorChar
         && separator != _realFileSystem.Path.AltDirectorySeparatorChar) {
            return null;
        }

        return NormalizeSeparators(fullPath[(RootPath.Length + 1)..])
            .TrimEnd(_realFileSystem.Path.DirectorySeparatorChar, _realFileSystem.Path.AltDirectorySeparatorChar);
    }

    /// <summary>
    /// Gets the archive file for the given full path inside this file system.
    /// </summary>
    /// <param name="fullPath">Full path inside this file system</param>
    /// <returns>The archive file, or null if it does not exist in the archive</returns>
    public IArchiveFile? GetArchiveFile(string fullPath) {
        var relativePath = GetArchiveRelativePath(fullPath);
        if (relativePath.IsNullOrEmpty()) return null;

        var directory = _realFileSystem.Path.GetDirectoryName(relativePath) ?? string.Empty;
        if (!Index.FilesByDirectory.TryGetValue(directory, out var files)) return null;

        return files.Find(file => DataRelativePath.PathComparer.Equals(file.Path, relativePath));
    }

    public bool FileExists(string fullPath) => GetArchiveFile(fullPath) is not null;

    public bool DirectoryExists(string fullPath) {
        var relativePath = GetArchiveRelativePath(fullPath);
        if (relativePath is null) return false;

        return Index.Directories.Contains(relativePath);
    }

    public IEnumerable<string> EnumerateFiles(string fullPath, string searchPattern, bool includeSubDirectories) {
        var relativePath = GetArchiveRelativePath(fullPath);
        if (relativePath is null) return [];

        return EnumerateFilesRelative(relativePath, searchPattern, includeSubDirectories);
    }

    public IEnumerable<string> EnumerateDirectories(string fullPath, string searchPattern, bool includeSubDirectories) {
        var relativePath = GetArchiveRelativePath(fullPath);
        if (relativePath is null) return [];

        return EnumerateDirectoriesRelative(relativePath, searchPattern, includeSubDirectories);
    }

    private IEnumerable<string> EnumerateFilesRelative(string relativeDirectory, string searchPattern, bool includeSubDirectories) {
        if (Index.FilesByDirectory.TryGetValue(relativeDirectory, out var files)) {
            foreach (var archiveFile in files) {
                if (!MatchesSearchPattern(_realFileSystem.Path.GetFileName(archiveFile.Path), searchPattern)) continue;

                yield return _realFileSystem.Path.Combine(RootPath, archiveFile.Path);
            }
        }

        if (!includeSubDirectories) yield break;

        foreach (var subDirectory in GetSubDirectories(relativeDirectory)) {
            foreach (var file in EnumerateFilesRelative(subDirectory, searchPattern, true)) {
                yield return file;
            }
        }
    }

    private IEnumerable<string> EnumerateDirectoriesRelative(string relativeDirectory, string searchPattern, bool includeSubDirectories) {
        foreach (var subDirectory in GetSubDirectories(relativeDirectory)) {
            if (!MatchesSearchPattern(_realFileSystem.Path.GetFileName(subDirectory), searchPattern)) continue;

            yield return _realFileSystem.Path.Combine(RootPath, subDirectory);

            if (!includeSubDirectories) continue;

            foreach (var nestedDirectory in EnumerateDirectoriesRelative(subDirectory, searchPattern, true)) {
                yield return nestedDirectory;
            }
        }
    }

    private IEnumerable<string> GetSubDirectories(string relativeDirectory) => Index.SubDirectoriesByDirectory.TryGetValue(relativeDirectory, out var subDirectories)
        ? subDirectories
        : [];

    private ArchiveIndex BuildIndex() {
        var filesByDirectory = new Dictionary<string, List<IArchiveFile>>(DataRelativePath.PathComparer);
        var directories = new SortedSet<string>(DataRelativePath.PathComparer) { string.Empty };

        foreach (var archiveFile in _archiveReader.Files) {
            var filePath = NormalizeSeparators(archiveFile.Path);
            var directory = _realFileSystem.Path.GetDirectoryName(filePath) ?? string.Empty;

            if (!filesByDirectory.TryGetValue(directory, out var files)) {
                files = [];
                filesByDirectory.Add(directory, files);
            }
            files.Add(archiveFile);

            // Register the directory of the file and all of its ancestor directories
            var currentDirectory = directory;
            while (!currentDirectory.IsNullOrEmpty()) {
                directories.Add(currentDirectory);
                currentDirectory = _realFileSystem.Path.GetDirectoryName(currentDirectory) ?? string.Empty;
            }
        }

        foreach (var files in filesByDirectory.Values) {
            files.Sort((x, y) => DataRelativePath.PathComparer.Compare(x.Path, y.Path));
        }

        // Map each directory to its direct subdirectories, keeping the sorted order of the directories
        var subDirectoriesByDirectory = new Dictionary<string, List<string>>(DataRelativePath.PathComparer);
        foreach (var directory in directories) {
            if (directory.IsNullOrEmpty()) continue;

            var parentDirectory = _realFileSystem.Path.GetDirectoryName(directory) ?? string.Empty;
            if (!subDirectoriesByDirectory.TryGetValue(parentDirectory, out var subDirectories)) {
                subDirectories = [];
                subDirectoriesByDirectory.Add(parentDirectory, subDirectories);
            }
            subDirectories.Add(directory);
        }

        return new ArchiveIndex(filesByDirectory, subDirectoriesByDirectory, directories);
    }

    private string NormalizeSeparators(string path)
        => path.Replace(_realFileSystem.Path.AltDirectorySeparatorChar, _realFileSystem.Path.DirectorySeparatorChar);

    private static bool MatchesSearchPattern(string fileName, string searchPattern) {
        if (searchPattern.IsNullOrEmpty() || searchPattern == "*") return true;

        return FileSystemName.MatchesSimpleExpression(searchPattern, fileName);
    }

    internal static NotSupportedException ReadOnly() => new("The archive file system is read-only.");
    internal static NotSupportedException NoDateTime() => new("The archive file system does not support file timestamps.");

    private sealed record ArchiveIndex(
        Dictionary<string, List<IArchiveFile>> FilesByDirectory,
        Dictionary<string, List<string>> SubDirectoriesByDirectory,
        SortedSet<string> Directories);
}
