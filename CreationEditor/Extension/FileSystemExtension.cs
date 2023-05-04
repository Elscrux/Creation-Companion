using System.IO.Abstractions;
using System.Security.Cryptography;
using Noggog;
namespace CreationEditor;

public static class FileSystemExtension {
    public static byte[] GetChecksum(this IFileSystem fileSystem, FilePath path) {
        using var md5 = MD5.Create();
        using var modFileStream = fileSystem.File.OpenRead(path);

        return md5.ComputeHash(modFileStream);
    }

    public static int GetChecksumBytesLength(this IFileSystem _) => MD5.HashSizeInBytes;
    
    public static bool IsChecksumValid(this IFileSystem fileSystem, FilePath path, byte[] checksum) {
        var actualChecksum = fileSystem.GetChecksum(path);
        return actualChecksum.SequenceEqual(checksum);
    }
}
