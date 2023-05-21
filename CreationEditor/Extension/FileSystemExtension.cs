using System.IO.Abstractions;
using System.Security.Cryptography;
using Noggog;
namespace CreationEditor;

public static class FileSystemExtension {
    public static byte[] GetFileChecksum(this IFileSystem fileSystem, string path) {
        using var md5 = MD5.Create();
        using var fileStream = fileSystem.File.OpenRead(path);

        return md5.ComputeHash(fileStream);
    }

    public static byte[] GetFileChecksum(this IFileSystem fileSystem, string path, MD5 md5) {
        using var fileStream = fileSystem.File.OpenRead(path);

        return md5.ComputeHash(fileStream);
    }

    public static bool IsFileChecksumValid(this IFileSystem fileSystem, string path, byte[] checksum) {
        var actualChecksum = fileSystem.GetFileChecksum(path);
        return actualChecksum.SequenceEqual(checksum);
    }

    public static byte[] GetDirectoryChecksum(this IFileSystem fileSystem, string directoryPath) {
        using var md5 = MD5.Create();
        var directoryInfo = fileSystem.DirectoryInfo.New(directoryPath);
        var checksumBytes = CalculateDirectoryChecksumBytes(directoryInfo);
        return checksumBytes;

        byte[] CalculateDirectoryChecksumBytes(IDirectoryInfo directory) {
            // Sort the files and directories to ensure consistent checksum across different runs
            var files = directory
                .GetFiles("*.*", SearchOption.AllDirectories)
                .OrderBy(p => p.FullName);
            var subDirectories = directory
                .GetDirectories("*.*", SearchOption.AllDirectories)
                .OrderBy(p => p.FullName);

            foreach (var file in files) {
                using var stream = file.OpenRead();
                var fileChecksum = md5.ComputeHash(stream);
                md5.TransformBlock(fileChecksum, 0, fileChecksum.Length, fileChecksum, 0);
            }

            // Mark the end of the hashing
            md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            foreach (var subDirectory in subDirectories) {
                var directoryChecksum = CalculateDirectoryChecksumBytes(subDirectory);
                md5.TransformBlock(directoryChecksum, 0, directoryChecksum.Length, directoryChecksum, 0);
            }

            // Mark the end of the hashing
            md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            return md5.Hash ?? Array.Empty<byte>();
        }
    }

    public static int GetChecksumBytesLength(this IFileSystem _) => MD5.HashSizeInBytes;

    public static long GetDirectorySize(this IFileSystem fileSystem, string path) {
        return GetDirectorySizeRec(fileSystem.DirectoryInfo.New(path));

        long GetDirectorySizeRec(IDirectoryInfo directoryInfo) {
            return directoryInfo.GetFiles().Sum(fi => fi.Length) +
                directoryInfo.GetDirectories().Select(GetDirectorySizeRec).Sum();
        }
    }
}
