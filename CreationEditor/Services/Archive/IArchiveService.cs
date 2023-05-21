using Mutagen.Bethesda.Archives;
namespace CreationEditor.Services.Archive;

public interface IArchiveService : IDisposable {
    string GetExtension();
    IArchiveReader GetReader(string path);
    IEnumerable<string> GetFilesInFolder(string path);
    IEnumerable<string> GetSubdirectories(string path);
}
