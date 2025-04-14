using CreationEditor.Resources.Comparer;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Archive;

public interface IArchiveProvider {
    /// <summary>
    /// The extension of the archive files this service can read.
    /// </summary>
    /// <returns>string of the file extension, including the starting dot</returns>
    string GetExtension();

    /// <summary>
    /// File links of archives ordered by their priority from low to high priority.
    /// </summary>
    IEnumerable<FileSystemLink> GetArchives();
}

public sealed class AllLoadedBSAArchiveProvider(
    IDataSourceService dataSourceService,
    IEditorEnvironment editorEnvironment)
    : IArchiveProvider {
    private const string BsaFilter = "*.bsa";

    public string GetExtension() => global::Mutagen.Bethesda.Archives.Archive.GetExtension(editorEnvironment.GameEnvironment.GameRelease);

    public IEnumerable<FileSystemLink> GetArchives() {
        // Collect bsa files in the data directory and sort them based on the load order
        var bsaNameOrder = editorEnvironment.GameEnvironment.LinkCache.ListedOrder
            .SelectMany(GetModBSAFiles)
            .ToArray();

        // todo potentially filter out bsa files that are not in the load order or referenced in the ini file
        var bsaNameComparer = new FuncComparer<FileSystemLink>((x, y) => {
            var indexOfX = bsaNameOrder.IndexOf(x.DataRelativePath.Path);
            var indexOfY = bsaNameOrder.IndexOf(y.DataRelativePath.Path);

            if (indexOfX == -1) {
                if (indexOfY == -1) return 0;

                return -1;
            }

            if (indexOfY == -1) return 1;

            // Files that are higher in the load order will have a higher index and be prioritized 
            return indexOfX.CompareTo(indexOfY);
        });

        return dataSourceService.PriorityOrder
            .Select(dataSource => new FileSystemLink(dataSource, string.Empty))
            .SelectMany(rootLink => rootLink.EnumerateFileLinks(BsaFilter, false))
            .Order(bsaNameComparer);
    }

    private IEnumerable<string> GetModBSAFiles(IModGetter mod) {
        var extension = GetExtension();

        yield return mod.ModKey.Name + extension;
        yield return mod.ModKey.Name + " - Textures" + extension;
    }
}
