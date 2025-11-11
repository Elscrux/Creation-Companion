using System.IO.Abstractions;
using System.IO.Hashing;
namespace CreationEditor;

public static class FileSystemExtension {
    extension(IFileSystem fileSystem) {
        public byte[] GetFileHash(string path) {
            using var fileStream = fileSystem.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            var hashAlgorithm = new XxHash3();
            hashAlgorithm.Append(fileStream);
            return hashAlgorithm.GetCurrentHash();
        }
        public bool IsFileHashValid(string path, byte[] hash) {
            var actualHash = fileSystem.GetFileHash(path);
            return actualHash.SequenceEqual(hash);
        }
        public int GetHashBytesLength() => 8;
        public long GetDirectorySize(string path) {
            return GetDirectorySizeRec(fileSystem.DirectoryInfo.New(path));

            long GetDirectorySizeRec(IDirectoryInfo directoryInfo) {
                return directoryInfo.GetFiles().Sum(fi => fi.Length) +
                    directoryInfo.GetDirectories().Select(GetDirectorySizeRec).Sum();
            }
        }
    }
}
