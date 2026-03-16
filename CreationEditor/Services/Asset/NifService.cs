using CreationEditor.Services.DataSource;
using nifly;
using Serilog;
namespace CreationEditor.Services.Asset;

public class NifService(ILogger logger) : IModelService {
    public bool HasCollision(DataSourceFileLink fileLink) {
        if (!fileLink.Exists()) return false;

        try {
            using var fileStream = fileLink.ReadFileStream();

            var nif = new NifFile();
            nif.Load(fileLink.FullPath);

            var niHeader = nif.GetHeader();
            using var blockCache = new niflycpp.BlockCache(niflycpp.BlockCache.SafeClone<NiHeader>(niHeader));
            for (uint blockId = 0; blockId < blockCache.Header.GetNumBlocks(); blockId++) {
                using var bhkCollisionObject = blockCache.EditableBlockById<bhkCollisionObject>(blockId);
                if (bhkCollisionObject is null) continue;

                return true;
            }
        } catch (Exception e) {
            logger.Here().Warning(e,
                "Failed to parse nif file {Path}: {Exception} for asset references",
                fileLink,
                e);
        }

        return false;
    }
}
