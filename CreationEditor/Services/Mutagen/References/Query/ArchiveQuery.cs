using CreationEditor.Services.Archive;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Parser;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class ArchiveQuery<TCache, TLink>(
    IFileParser<TLink> fileParser,
    IArchiveService archiveService)
    : IReferenceQuery<IDataSource, TCache, TLink, DataRelativePath>
    where TCache : IReferenceCache<TCache, TLink, DataRelativePath>
    where TLink : notnull {
    public string Name => fileParser.Name + " (Archive)";
    public string GetSourceName(IDataSource source) => source.Name;
    public IDataSource? ReferenceToSource(DataRelativePath reference) => null;
    public void FillCache(IDataSource source, TCache cache) {
        var archiveReader = archiveService.GetReader(source.GetRootLink());

        foreach (var archiveFile in archiveReader.Files) {
            var extension = source.FileSystem.Path.GetExtension(archiveFile.Path);
            if (!fileParser.FileExtensions.Contains(extension, DataRelativePath.PathComparer)) continue;

            var fileSystem = source.FileSystem;
            var tempPath = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetRandomFileName());
            try {
                //Create a temp file and copy the file from the archive to it
                using var archiveStream = fileSystem.File.Create(tempPath);
                archiveFile.AsStream().CopyTo(archiveStream);
                archiveStream.Close();

                //Parse the temp file as nif and delete it afterward
                foreach (var result in fileParser.ParseFile(tempPath, fileSystem)) {
                    cache.Add(result, archiveFile.Path);
                }
            } finally {
                fileSystem.File.Delete(tempPath);
            }
        }
    }
}
