using CreationEditor.Services.DataSource;
using NiflySharp;
using NiflySharp.Blocks;
using Serilog;
namespace CreationEditor.Services.Asset;

public interface IModelService {
    bool HasCollision(DataSourceFileLink fileLink);
}

public class NifService(ILogger logger) : IModelService {
    public bool HasCollision(DataSourceFileLink fileLink) {
        if (!fileLink.Exists()) return false;

        try {
            using var fileStream = fileLink.ReadFileStream();
            var nif = new NifFile(fileStream);

            return nif.Blocks.OfType<bhkCollisionObject>().Any();
        } catch (Exception e) {
            logger.Here().Warning(e,
                "Failed to parse nif file {Path}: {Exception} for asset references",
                fileLink,
                e.Message);
        }

        return false;
    }
}
