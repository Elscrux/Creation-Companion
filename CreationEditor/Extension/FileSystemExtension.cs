using System.IO.Abstractions;
using System.Security.Cryptography;
using Noggog;
namespace CreationEditor.Extension; 

public static class FileSystemExtension {
    public static byte[] GetChecksum(this IFileSystem fileSystem, FilePath path) {
        using var md5 = MD5.Create();
        using var modFileStream = fileSystem.File.OpenRead(path);
        
        return md5.ComputeHash(modFileStream);
    }
    
    public static int GetChecksumBytesLength(this IFileSystem _) => MD5.HashSizeInBytes;
}
