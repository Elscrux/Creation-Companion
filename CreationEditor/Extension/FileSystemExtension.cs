using System.IO.Abstractions;
using System.IO.Hashing;
namespace CreationEditor;

public static class FileSystemExtension {
    public static byte[] GetFileHash(this IFileSystem fileSystem, string path) {
        using var fileStream = fileSystem.File.OpenRead(path);

        var hashAlgorithm = new XxHash3();
        hashAlgorithm.Append(fileStream);
        return hashAlgorithm.GetCurrentHash();
    }

    public static bool IsFileHashValid(this IFileSystem fileSystem, string path, byte[] hash) {
        var actualHash = fileSystem.GetFileHash(path);
        return actualHash.SequenceEqual(hash);
    }

    public static int GetHashBytesLength(this IFileSystem _) => 8;

    public static long GetDirectorySize(this IFileSystem fileSystem, string path) {
        return GetDirectorySizeRec(fileSystem.DirectoryInfo.New(path));

        long GetDirectorySizeRec(IDirectoryInfo directoryInfo) {
            return directoryInfo.GetFiles().Sum(fi => fi.Length) +
                directoryInfo.GetDirectories().Select(GetDirectorySizeRec).Sum();
        }
    }
}
