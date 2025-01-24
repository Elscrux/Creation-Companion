using System.IO.Abstractions;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public abstract class AssetFilter<T>(ILinkCacheProvider linkCacheProvider, IFileSystem fileSystem) : IRecordFilter {
    public Type Type => typeof(T);

    public IEnumerable<RecordFilterListing> GetListings(Type type) {
        var recordFilterListings = linkCacheProvider.LinkCache.PriorityOrder.WinningOverrides(type)
            .OfType<T>()
            .GetRecursiveListings(GetPaths, fileSystem.Path.DirectorySeparatorChar, fileSystem.Path.AltDirectorySeparatorChar)
            .ToList();

        // Trim standalone folders
        while (recordFilterListings.Count == 1) {
            recordFilterListings = recordFilterListings[0].RecordFilters.ToList();
        }

        return recordFilterListings;
    }

    protected virtual IEnumerable<string> GetPaths(T record) {
        if (record is not IAssetLinkContainerGetter assetLinkContainer) yield break;

        foreach (var assetLink in assetLinkContainer.EnumerateListedAssetLinks()) {
            yield return assetLink.DataRelativePath.Path;
        }
    }
}
