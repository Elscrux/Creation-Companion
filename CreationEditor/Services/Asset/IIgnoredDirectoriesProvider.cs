using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Asset;

public interface IIgnoredDirectoriesProvider {
    /// <summary>
    /// Checks if a path should be ignored.
    /// </summary>
    /// <returns>If the path is supposed to be ignored</returns>
    bool IsIgnored(DataRelativePath path);

    /// <summary>
    /// Adds a directory to the list of ignored directories.
    /// </summary>
    /// <param name="directory">The directory path to add to the ignored list.</param>
    void AddIgnoredDirectory(DataRelativePath directory);
}
