using System.IO.Abstractions;
using System.Security.Cryptography;
using Noggog;
namespace CreationEditor;

public static class FileSystemExtension {
    private const string WildcardSearchPattern = "*";

    public static byte[] GetFileHash(this IFileSystem fileSystem, string path) {
        using var md5 = MD5.Create();
        using var fileStream = fileSystem.File.OpenRead(path);

        return md5.ComputeHash(fileStream);
    }

    public static byte[] GetFileHash(this IFileSystem fileSystem, string path, MD5 md5) {
        using var fileStream = fileSystem.File.OpenRead(path);

        return md5.ComputeHash(fileStream);
    }

    public static bool IsFileHashValid(this IFileSystem fileSystem, string path, byte[] hash) {
        var actualHash = fileSystem.GetFileHash(path);
        return actualHash.SequenceEqual(hash);
    }

    public static byte[] GetDirectoryHash(this IFileSystem fileSystem, string directoryPath) {
        using var md5 = MD5.Create();
        var directoryInfo = fileSystem.DirectoryInfo.New(directoryPath);
        var hashBytes = CalculateDirectoryHashBytes(directoryInfo);
        return hashBytes;

        byte[] CalculateDirectoryHashBytes(IDirectoryInfo directory) {
            // Sort the files and directories to ensure consistent hash across different runs
            var files = directory
                .GetFiles(WildcardSearchPattern, SearchOption.AllDirectories)
                .OrderBy(p => p.FullName);
            var subDirectories = directory
                .GetDirectories(WildcardSearchPattern, SearchOption.AllDirectories)
                .OrderBy(p => p.FullName);

            foreach (var file in files) {
                using var stream = file.OpenRead();
                var fileHash = md5.ComputeHash(stream);
                md5.TransformBlock(fileHash, 0, fileHash.Length, fileHash, 0);
            }

            // Mark the end of the hashing
            md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            foreach (var subDirectory in subDirectories) {
                var directoryHash = CalculateDirectoryHashBytes(subDirectory);
                md5.TransformBlock(directoryHash, 0, directoryHash.Length, directoryHash, 0);
            }

            // Mark the end of the hashing
            md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            return md5.Hash ?? Array.Empty<byte>();
        }
    }

    public static int GetHashBytesLength(this IFileSystem _) => MD5.HashSizeInBytes;

    public static long GetDirectorySize(this IFileSystem fileSystem, string path) {
        return GetDirectorySizeRec(fileSystem.DirectoryInfo.New(path));

        long GetDirectorySizeRec(IDirectoryInfo directoryInfo) {
            return directoryInfo.GetFiles().Sum(fi => fi.Length) +
                directoryInfo.GetDirectories().Select(GetDirectorySizeRec).Sum();
        }
    }
}
