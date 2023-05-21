using Autofac;
using CreationEditor.Services.Archive;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public abstract class FileStructureAssetQuery : AssetQuery<string, string> {
    protected readonly IArchiveService ArchiveService;

    protected FileStructureAssetQuery(ILifetimeScope lifetimeScope) : base(lifetimeScope) {
        var newScope = lifetimeScope.BeginLifetimeScope().DisposeWith(this);
        ArchiveService = newScope.Resolve<IArchiveService>();

        // BSAParsing = bsaParsing;
    }

    public override IEnumerable<AssetQueryResult<string>> ParseAssets(string directory) {
        foreach (var file in FileSystem.Directory.EnumerateFiles(directory)) {
            foreach (var result in ParseFile(file)) {
                yield return result;
            }
        }

        foreach (var subdirectory in FileSystem.Directory.GetDirectories(directory)) {
            foreach (var result in ParseAssets(subdirectory)) {
                yield return result;
            }
        }
    }

    public abstract IEnumerable<AssetQueryResult<string>> ParseFile(string file);

    protected override void WriteCacheCheck(BinaryWriter writer, string origin) {
        var directoryInfo = FileSystem.DirectoryInfo.New(origin);

        writer.Write(directoryInfo.LastWriteTimeUtc.Ticks);
        writer.Write(FileSystem.GetDirectorySize(directoryInfo.FullName));
    }

    protected override void WriteContext(BinaryWriter writer, string origin) => writer.Write(origin);

    protected override void WriteUsages(BinaryWriter writer, IEnumerable<string> usages) {
        foreach (var usage in usages) {
            writer.Write(usage);
        }
    }

    protected override bool IsCacheUpToDate(BinaryReader reader, string origin) {
        var directoryInfo = FileSystem.DirectoryInfo.New(origin);

        var time = reader.ReadInt64();
        if (directoryInfo.LastWriteTimeUtc.Ticks != time) return false;

        var size = reader.ReadInt64();
        var actualSize = FileSystem.GetDirectorySize(directoryInfo.FullName);
        if (actualSize != size) return false;

        return true;
    }

    protected override string ReadContextString(BinaryReader reader) => reader.ReadString();

    protected override IEnumerable<string> ReadUsages(BinaryReader reader, string context, int count) {
        for (var i = 0; i < count; i++) {
            yield return reader.ReadString();
        }
    }
}
