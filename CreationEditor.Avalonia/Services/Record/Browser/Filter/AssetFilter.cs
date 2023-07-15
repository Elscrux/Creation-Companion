using System.IO.Abstractions;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public abstract class AssetFilter<T> : IRecordFilter {
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IFileSystem _fileSystem;

    public Type Type => typeof(T);

    protected AssetFilter(
        IEditorEnvironment editorEnvironment,
        IFileSystem fileSystem) {
        _editorEnvironment = editorEnvironment;
        _fileSystem = fileSystem;
    }

    public IEnumerable<RecordFilterListing> GetListings(Type type) {
        var recordFilterListings = _editorEnvironment.LinkCache.PriorityOrder.WinningOverrides(type)
            .OfType<T>()
            .GetRecursiveListings(GetPaths, _fileSystem.Path.DirectorySeparatorChar, _fileSystem.Path.AltDirectorySeparatorChar)
            .ToList();

        // Trim standalone folders
        while (recordFilterListings.Count == 1) {
            recordFilterListings = recordFilterListings.First().RecordFilters.ToList();
        }

        return recordFilterListings;
    }

    protected virtual IEnumerable<string> GetPaths(T record) {
        if (record is not IAssetLinkContainerGetter assetLinkContainer) yield break;

        foreach (var assetLink in assetLinkContainer.EnumerateListedAssetLinks()) {
            yield return assetLink.DataRelativePath;
        }
    }
}
