using System.IO.Abstractions;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Parser;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using NiflySharp;
using NiflySharp.Blocks;
using Serilog;
namespace CreationEditor.Services.Asset;

public sealed class NifService(
    ILogger logger,
    IFileSystem fileSystem,
    NifTextureParser nifTextureParser) : IModelService {
    public bool HasCollision(DataSourceFileLink fileLink) {
        if (!fileLink.Exists()) return false;

        try {
            using var fileStream = fileLink.ReadFileStream();

            var nif = new NifFile(fileStream);

            return nif.Blocks.OfType<bhkCollisionObject>().Any();
        } catch (Exception e) {
            logger.Here().Warning(e, "Failed to parse nif file {Path} for asset references", fileLink);
        }

        return false;
    }

    public IEnumerable<string> GetMisalignedPaths(DataSourceFileLink fileLink) {
        using var stream = fileLink.ReadFileStream();
        if (stream is null) yield break;

        foreach (var textureString in nifTextureParser.ParseFileTextureStrings(stream, fileLink)) {
            string? misalignedPath = null;

            try {
                _ = new DataRelativePath(textureString);
            } catch (AssetPathMisalignedException e) {
                misalignedPath = e.Path;
            }

            if (misalignedPath is not null) {
                yield return misalignedPath;
            }
        }
    }
}
