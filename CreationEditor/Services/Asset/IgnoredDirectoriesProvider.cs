using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Asset;

public class IgnoredDirectoriesProvider : IIgnoredDirectoriesProvider {
    private readonly HashSet<DataRelativePath> _ignoredDirectories = [".git"];

    public bool IsIgnored(DataRelativePath path) {
        // Check if the path starts with any of the ignored directories
        foreach (var ignoredDirectory in _ignoredDirectories) {
            if (path.Path.StartsWith(ignoredDirectory.Path, DataRelativePath.PathComparison)) {
                return true;
            }
        }

        return false;
    }

    public void AddIgnoredDirectory(DataRelativePath directory) {
        _ignoredDirectories.Add(directory);
    }
}
