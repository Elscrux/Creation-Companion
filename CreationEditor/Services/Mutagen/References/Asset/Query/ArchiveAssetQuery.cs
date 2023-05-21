using Autofac;
using CreationEditor.Services.Archive;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public abstract class ArchiveAssetQuery : AssetQuery<string, string> {
    protected readonly IArchiveService ArchiveService;

    protected ArchiveAssetQuery(ILifetimeScope lifetimeScope) : base(lifetimeScope) {
        var newScope = lifetimeScope.BeginLifetimeScope().DisposeWith(this);
        ArchiveService = newScope.Resolve<IArchiveService>();
    }

    protected override void WriteCacheCheck(BinaryWriter writer, string origin) {
        var checksum = FileSystem.GetFileChecksum(origin);
        writer.Write(checksum);
    }

    protected override void WriteContext(BinaryWriter writer, string origin) => writer.Write(origin);

    protected override void WriteUsages(BinaryWriter writer, IEnumerable<string> usages) {
        foreach (var usage in usages) {
            writer.Write(usage);
        }
    }

    protected override bool IsCacheUpToDate(BinaryReader reader, string origin) {
        var checksum = reader.ReadBytes(FileSystem.GetChecksumBytesLength());
        return FileSystem.IsFileChecksumValid(origin, checksum);
    }

    protected override string ReadContextString(BinaryReader reader) => reader.ReadString();

    protected override IEnumerable<string> ReadUsages(BinaryReader reader, string context, int count) {
        for (var i = 0; i < count; i++) {
            yield return reader.ReadString();
        }
    }
}
