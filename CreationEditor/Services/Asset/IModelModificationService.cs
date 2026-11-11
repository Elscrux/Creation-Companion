using CreationEditor.Services.DataSource;
namespace CreationEditor.Services.Asset;

public interface IModelModificationService {
    /// <summary>
    /// Remaps links in a file based on a function that transforms the old link to a new link.
    /// </summary>
    /// <param name="fileLink">File system link to remap links for</param>
    /// <param name="replace">FUnction that receives a textures and returns a replacement texture (or the same texture if it should not be replaced</param>
    void RemapLinks(DataSourceFileLink fileLink, Func<string, string> replace);

    /// <summary>
    /// Determines whether the service can accept a given file link for modification.
    /// </summary>
    /// <param name="fileLink">File system link to check</param>
    /// <returns> True if the service can accept the file link, false otherwise</returns>
    bool Accepts(DataSourceFileLink fileLink);
}
