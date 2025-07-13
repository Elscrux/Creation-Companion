using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Asset;

public interface IModelModificationService {
    /// <summary>
    /// Remaps links in a file based on a predicate to another link.
    /// </summary>
    /// <param name="fileLink">File system link to remap links for</param>
    /// <param name="shouldReplaceLink">Function that decides if a link should be replaced with the new link</param>
    /// <param name="newLink">New link to replace the old link with</param>
    void RemapLinks(DataSourceFileLink fileLink, Func<string, bool> shouldReplaceLink, DataRelativePath newLink);
}
